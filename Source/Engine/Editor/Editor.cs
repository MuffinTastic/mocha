global using ImGuiNET;

namespace Mocha.Editor;

public class Editor
{
	static bool drawPerformanceOverlay = false;

	public static List<EditorWindow> EditorWindows = new()
	{
		// new MaterialEditorWindow(),
		new ConsoleWindow(),
		new BrowserWindow()
		// new MemoryWindow()
	};

	public static void Draw()
	{
		DrawMenuBar();
		DrawStatusBar();

		if ( drawPerformanceOverlay )
			DrawPerformanceOverlay();

		foreach ( var window in EditorWindows.ToArray() )
		{
			if ( window.isVisible )
				window.Draw();
		}
	}

	private static void DrawMenuBar()
	{
		using ( ImGuiX.Scope menuBarScope = ImGuiX.MainMenuBar() )
		{
			if ( !menuBarScope.Visible )
				return;

			ImGui.Dummy( new Vector2( 4, 0 ) );
			ImGui.Text( "Mocha Engine" );
			ImGui.Dummy( new Vector2( 4, 0 ) );

			ImGui.Separator();
			ImGui.Dummy( new Vector2( 4, 0 ) );

			using ( ImGuiX.Scope windowMenuScope = ImGuiX.Menu( "Window" ) )
			{
				if ( windowMenuScope.Visible )
				{
					foreach ( var window in EditorWindows )
					{
						var displayInfo = DisplayInfo.For( window );
						if ( ImGui.MenuItem( displayInfo.Name ) )
							window.isVisible = !window.isVisible;
					}

					if ( ImGui.MenuItem( "Performance Overlay" ) )
						drawPerformanceOverlay = !drawPerformanceOverlay;
				}
			}

			ImGuiX.RenderViewDropdown();
		}
	}

	private static void DrawStatusBar()
	{
		using ( ImGuiX.Scope scope = ImGuiX.MainStatusBar() )
		{
			if ( !scope.Visible )
				return;

			ImGui.Text( $"{Screen.Size.X}x{Screen.Size.Y}" );

			ImGui.Dummy( new Vector2( 4, 0 ) );
			ImGui.Separator();
			ImGui.Dummy( new Vector2( 4, 0 ) );
			ImGui.Text( $"{Time.FPS} FPS" );

			// Filler
			var windowWidth = ImGui.GetWindowWidth();
			var cursorX = ImGui.GetCursorPosX();
			ImGui.Dummy( new Vector2( windowWidth - cursorX - 150f, 0 ) );

			ImGui.Separator();
			ImGui.Dummy( new Vector2( 4, 0 ) );
			ImGui.Text( "Press ~ to toggle cursor" );
		}
	}

	private static void DrawPerformanceOverlay()
	{
		using ( ImGuiX.Scope scope = ImGuiX.Overlay( "Time" ) )
		{
			if ( !scope.Visible )
				return;

			var gpuName = ImGuiX.GetGPUName();

			ImGui.Text( $"GPU: {gpuName}" );
			ImGui.Text( $"FPS: {Time.FPS}" );
			ImGui.Text( $"Current time: {Time.Now}" );
			ImGui.Text( $"Frame time: {(Time.Delta * 1000f).CeilToInt()}ms" );
		}
	}
}
