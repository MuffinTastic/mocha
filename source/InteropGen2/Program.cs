﻿public static class Program
{
	internal static List<string> GeneratedPaths { get; set; } = new();
	internal static List<IUnit> Units { get; set; } = new();
	internal static List<string> Files { get; set; } = new();

	private static void ProcessHeader( string baseDir, string path )
	{
		Console.WriteLine( $"\t Processing header {path}" );

		var units = Parser.GetUnits( path );
		var fileName = Path.GetFileNameWithoutExtension( path );

		var managedGenerator = new ManagedCodeGenerator( units );
		var managedCode = managedGenerator.GenerateManagedCode();
		File.WriteAllText( $"{baseDir}/Common/Glue/{fileName}.generated.cs", managedCode );

		var nativeGenerator = new NativeCodeGenerator( units );
		var relativePath = Path.GetRelativePath( $"{baseDir}/Host/", path );
		var nativeCode = nativeGenerator.GenerateNativeCode( relativePath );

		Console.WriteLine( $"{baseDir}/Host/generated/{fileName}.generated.h" );
		File.WriteAllText( $"{baseDir}/Host/generated/{fileName}.generated.h", nativeCode );

		Files.Add( fileName );
		Units.AddRange( units );
	}

	private static void QueueDirectory( ref List<string> queue, string directory )
	{
		foreach ( var file in Directory.GetFiles( directory ) )
		{
			if ( file.EndsWith( ".h" ) && !file.EndsWith( ".generated.h" ) )
			{
				var fileContents = File.ReadAllText( file );

				if ( !fileContents.Contains( "GENERATE_BINDINGS", StringComparison.CurrentCultureIgnoreCase ) )
					continue; // Fast early bail

				QueueFile( ref queue, file );
			}
		}

		foreach ( var subDirectory in Directory.GetDirectories( directory ) )
		{
			QueueDirectory( ref queue, subDirectory );
		}
	}

	private static void QueueFile( ref List<string> queue, string path )
	{
		queue.Add( path );
	}

	public static void Main( string[] args )
	{
		var start = DateTime.Now;
		Console.WriteLine( "Generating C# <--> C++ interop code..." );

		var destCsDir = $"{args[0]}\\Common\\Glue\\";
		var destHeaderDir = $"{args[0]}\\Host\\generated\\";

		if ( Directory.Exists( destHeaderDir ) )
			Directory.Delete( destHeaderDir, true );
		if ( Directory.Exists( destCsDir ) )
			Directory.Delete( destCsDir, true );

		Directory.CreateDirectory( destHeaderDir );
		Directory.CreateDirectory( destCsDir );

		List<string> queue = new();
		QueueDirectory( ref queue, args[0] );

		int completedThreads = 0;

		var dispatcher = new Mocha.Common.ThreadDispatcher<string>( ( files ) =>
		{
			foreach ( var path in files )
			{
				ProcessHeader( args[0], path );
			}

			completedThreads++;
		}, queue );

		while ( !dispatcher.IsComplete )
			Thread.Sleep( 500 );

		// Expand methods out into list of (method name, method)
		var methods = Units.OfType<Class>().SelectMany( unit => unit.Methods, ( unit, method ) => (unit.Name, method) ).ToList();

		//
		// Write managed struct
		//
		{
			var (baseManagedStructWriter, managedStructWriter) = Utils.CreateWriter();

			managedStructWriter.WriteLine( $"using System.Runtime.InteropServices;" );
			managedStructWriter.WriteLine();
			managedStructWriter.WriteLine( $"[StructLayout( LayoutKind.Sequential )]" );
			managedStructWriter.WriteLine( $"public struct UnmanagedArgs" );
			managedStructWriter.WriteLine( $"{{" );

			managedStructWriter.Indent++;

			var managedStructBody = string.Join( "\r\n\t", methods.Select( x => $"public IntPtr __{x.Name}_{x.method.Name}MethodPtr;" ) );
			managedStructWriter.Write( managedStructBody );
			managedStructWriter.WriteLine();

			managedStructWriter.Indent--;

			managedStructWriter.WriteLine( $"}}" );
			managedStructWriter.Dispose();

			File.WriteAllText( $"{args[0]}/Common/Glue/UnmanagedArgs.cs", baseManagedStructWriter.ToString() );
		}

		//
		// Write native struct
		//
		{
			var (baseNativeStructWriter, nativeStructWriter) = Utils.CreateWriter();

			nativeStructWriter.WriteLine( "#ifndef __GENERATED_UNMANAGED_ARGS_H" );
			nativeStructWriter.WriteLine( "#define __GENERATED_UNMANAGED_ARGS_H" );
			nativeStructWriter.WriteLine( "#include \"InteropList.generated.h\"" );
			nativeStructWriter.WriteLine();
			nativeStructWriter.WriteLine( "struct UnmanagedArgs" );
			nativeStructWriter.WriteLine( $"{{" );
			nativeStructWriter.Indent++;

			var nativeStructBody = string.Join( "\r\n\t", methods.Select( x => $"void* __{x.Name}_{x.method.Name}MethodPtr;" ) );
			nativeStructWriter.Write( nativeStructBody );
			nativeStructWriter.WriteLine();

			nativeStructWriter.Indent--;
			nativeStructWriter.WriteLine( $"}};" );
			nativeStructWriter.WriteLine();

			nativeStructWriter.WriteLine( "inline UnmanagedArgs args" );
			nativeStructWriter.WriteLine( $"{{" );
			nativeStructWriter.Indent++;

			nativeStructBody = string.Join( ",\r\n\t", methods.Select( x => $"(void*)__{x.Name}_{x.method.Name}" ) );
			nativeStructWriter.Write( nativeStructBody );
			nativeStructWriter.WriteLine();

			nativeStructWriter.Indent--;
			nativeStructWriter.WriteLine( $"}};" );

			nativeStructWriter.WriteLine();
			nativeStructWriter.WriteLine( $"#endif // __GENERATED_UNMANAGED_ARGS_H" );
			nativeStructWriter.Dispose();

			File.WriteAllText( $"{args[0]}/Host/generated/UnmanagedArgs.generated.h", baseNativeStructWriter.ToString() );
		}

		//
		// Write native includes
		//
		{
			var (baseNativeListWriter, nativeListWriter) = Utils.CreateWriter();

			nativeListWriter.WriteLine( "#ifndef __GENERATED_INTEROPLIST_H" );
			nativeListWriter.WriteLine( "#define __GENERATED_INTEROPLIST_H" );
			nativeListWriter.WriteLine();
			nativeListWriter.Indent++;

			var nativeListBody = string.Join( "\r\n\t", Files.Select( x => $"#include \"{x}.generated.h\"" ) );
			nativeListWriter.Write( nativeListBody );
			nativeListWriter.WriteLine();

			nativeListWriter.Indent--;
			nativeListWriter.WriteLine();
			nativeListWriter.WriteLine( "#endif // __GENERATED_INTEROPLIST_H" );

			File.WriteAllText( $"{args[0]}/Host/generated/InteropList.generated.h", baseNativeListWriter.ToString() );
		}

		var end = DateTime.Now;

		var totalTime = (end - start);
		Console.WriteLine( $"-- Took {totalTime.TotalSeconds} seconds." );
	}
}
