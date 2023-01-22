namespace Mocha.Editor;

public static class ImGuiX
{
	public enum ScopeType
	{
		Window,
		Child,
		Disabled,
		ListBox,
		MainStatusBar,
		MainMenuBar,
		Menu,
		Popup,
		TabBar,
		TabItem,
		Table
	}

	public struct Scope : IDisposable
	{
		public required ScopeType Type;
		public required bool Visible;

		// ImGui is a bit inconsistent, in that EndChild has to be
		// called regardless of the whether or not the child scope
		// is visible.
		public required bool CallEnd;

		public void Dispose()
		{
			if ( !CallEnd )
				return;

			switch ( Type )
			{
				case ScopeType.Window:
					ImGui.End();
					break;

				case ScopeType.Child:
					ImGui.EndChild();
					break;

				case ScopeType.Disabled:
					ImGui.EndDisabled();
					break;

				case ScopeType.ListBox:
					ImGui.EndListBox();
					break;

				case ScopeType.MainStatusBar:
					EndMainStatusBar();
					break;

				case ScopeType.MainMenuBar:
					ImGui.EndMainMenuBar();
					break;

				case ScopeType.Menu:
					ImGui.EndMenu();
					break;

				case ScopeType.Popup:
					ImGui.EndPopup();
					break;

				case ScopeType.TabBar:
					ImGui.EndTabItem();
					break;

				case ScopeType.TabItem:
					ImGui.EndTabItem();
					break;

				case ScopeType.Table:
					ImGui.EndTable();
					break;
			}
		}
	}

	public static Scope Begin( string name, ImGuiWindowFlags flags )
	{
		bool visible = ImGui.Begin( name, flags );

		return new Scope
		{
			Type = ScopeType.Window,
			Visible = visible,
			CallEnd = visible
		};
	}

	public static Scope Begin( string name, ref bool isOpen, ImGuiWindowFlags flags )
	{
		bool visible = ImGui.Begin( name, ref isOpen, flags );

		return new Scope
		{
			Type = ScopeType.Window,
			Visible = visible,
			CallEnd = visible
		};
	}

	public static Scope Child( string name, Vector2 size )
	{
		bool visible = ImGui.BeginChild( name, size );

		return new Scope
		{
			Type = ScopeType.Child,
			Visible = visible,
			CallEnd = true
		};
	}

	public static Scope Child( string name, Vector2 size, bool border, ImGuiWindowFlags flags )
	{
		bool visible = ImGui.BeginChild( name, size, border, flags );

		return new Scope
		{
			Type = ScopeType.Child,
			Visible = visible,
			CallEnd = true
		};
	}

	public static Scope ListBox( string label, Vector2 size )
	{
		bool visible = ImGui.BeginListBox( label, size );

		return new Scope
		{
			Type = ScopeType.ListBox,
			Visible = visible,
			CallEnd = visible
		};
	}

	public static Scope MainMenuBar()
	{
		bool visible = ImGui.BeginMainMenuBar();

		return new Scope
		{
			Type = ScopeType.MainMenuBar,
			Visible = visible,
			CallEnd = visible
		};
	}

	public static Scope Popup( string name, ImGuiWindowFlags flags )
	{
		bool visible = ImGui.BeginPopup( name, flags);

		return new Scope
		{
			Type = ScopeType.Popup,
			Visible = visible,
			CallEnd = visible
		};
	}

	public static Scope TabBar( string label )
	{
		bool visible = ImGui.BeginTabBar( label );

		return new Scope
		{
			Type = ScopeType.TabBar,
			Visible = visible,
			CallEnd = visible
		};
	}

	public static Scope BeginTabItem( string label )
	{
		bool visible = ImGui.BeginTabItem( label );

		return new Scope
		{
			Type = ScopeType.TabItem,
			Visible = visible,
			CallEnd = visible
		};
	}

	public static Scope Table( string name, int column, ImGuiTableFlags flags )
	{
		bool visible = ImGui.BeginTable( name, column, flags );

		return new Scope
		{
			Type = ScopeType.Table,
			Visible = visible,
			CallEnd = visible
		};
	}

	public static Scope DisabledIf( bool disabled )
	{
		if ( disabled )
			ImGui.BeginDisabled();

		return new Scope
		{
			Type = ScopeType.Disabled,
			Visible = true,
			CallEnd = disabled
		};
	}

	public static void SeparatorH()
	{
		ImGui.Dummy( new Vector2( 0, 4 ) );
		ImGui.PushStyleColor( ImGuiCol.Separator, new Vector4( 0.28f, 0.28f, 0.28f, 0.29f ) );
		ImGui.Separator();
		ImGui.PopStyleColor();
		ImGui.Dummy( new Vector2( 0, 4 ) );
	}
	public static void SeparatorV()
	{
		ImGui.Dummy( new Vector2( 8, 0 ) );
		ImGui.SameLine();
	}

	public static void BumpCursorX( float x )
	{
		float curr = ImGui.GetCursorPosX();
		ImGui.SetCursorPosX( curr + x );
	}

	public static void BumpCursorY( float y )
	{
		float curr = ImGui.GetCursorPosY();
		ImGui.SetCursorPosY( curr + y );
	}

	public static Scope Window( string name, ref bool isOpen )
	{
		var flags = ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.AlwaysUseWindowPadding |
					ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoScrollbar |
					ImGuiWindowFlags.NoResize;

		Scope scope = Begin( name, ref isOpen, flags );

		if ( scope.Visible )
		{
			if ( ImGui.GetWindowViewport().ID != ImGui.GetMainViewport().ID )
			{
				// TODO: This window is its own thing - let's give it an icon
			}
		}

		return scope;
	}

	public static void TextMonospace( string text )
	{
		Glue.Editor.TextMonospace( text );
	}

	public static void TextLight( string text )
	{
		Glue.Editor.TextLight( text );
	}

	public static void TextBold( string text )
	{
		Glue.Editor.TextBold( text );
	}

	public static void TextHeading( string text )
	{
		Glue.Editor.TextHeading( text );
	}

	public static void TextSubheading( string text )
	{
		Glue.Editor.TextSubheading( text );
	}

	public static Scope MainStatusBar()
	{
		bool visible = Glue.Editor.BeginMainStatusBar();

		return new Scope
		{
			Type = ScopeType.MainStatusBar,
			Visible = visible,
			CallEnd = visible
		};
	}

	public static void EndMainStatusBar()
	{
		ImGui.EndMenuBar();
		ImGui.End();
	}

	public static Scope Overlay( string name )
	{
		ImGui.SetNextWindowViewport( ImGui.GetMainViewport().ID );

		Scope scope = Begin( name, ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoInputs );

		if ( scope.Visible )
		{
			Vector2 workPos = ImGui.GetMainViewport().WorkPos;

			ImGui.SetWindowPos( new Vector2( workPos.X + 16, workPos.Y + 16 ) );
			ImGui.SetWindowSize( new Vector2( -1, -1 ) );
		}

		return scope;
	}

	public static string GetGPUName()
	{
		return Glue.Editor.GetGPUName();
	}

	public static void RenderViewDropdown()
	{
		Glue.Editor.RenderViewDropdown();
	}
	public static string Align( string str ) => str.PadRight( 16, ' ' );

	public static void DrawColoredText( string str, System.Numerics.Vector4 col, bool align = true )
	{
		ImGui.PushStyleColor( ImGuiCol.Text, col );

		if ( align )
			str = Align( str );
		ImGui.Text( str );

		ImGui.PopStyleColor();
	}

	public static void ApplyPadding()
	{
		var padding = new System.Numerics.Vector2( 4, 2 );
		SetCursorPosRelative( padding );
	}

	public static bool ImageButton( Texture texture, Vector2 size )
	{
		return false;
	}

	public static void Image( Texture texture, Vector2 size )
	{
		Glue.Editor.Image( texture.NativeTexture, texture.Width, texture.Height, (int)size.X, (int)size.Y );
	}

	public static void Image( Texture texture, Vector2 size, Vector4 tint )
	{
		Glue.Editor.Image( texture.NativeTexture, texture.Width, texture.Height, (int)size.X, (int)size.Y );
	}

	public static void Image( string texturePath, Vector2 size )
	{
		Image( Texture.FromCache( texturePath ), size );
	}

	public static void Image( string texture, Vector2 size, Vector4 tint )
	{
		Image( Texture.FromCache( texture ), size, tint );
	}

	public static void Separator()
	{
		ImGui.Dummy( new( 0, 4 ) );
		ImGui.PushStyleColor( ImGuiCol.Separator, new System.Numerics.Vector4( 0.28f, 0.28f, 0.28f, 0.29f ) );
		ImGui.Separator();
		ImGui.PopStyleColor();
		ImGui.Dummy( new( 0, 4 ) );
	}

	public static string GetDisplayName( string name )
	{
		string str = "";

		for ( int i = 0; i < name.Length; ++i )
		{
			char c = name[i];
			if ( i != 0 && char.IsUpper( c ) )
				str += " ";

			str += c;
		}

		return str;
	}

	public static bool Vector3Input( string v, ref System.Numerics.Vector3 sysVec3 )
	{
		float x = sysVec3.X;
		float y = sysVec3.Y;
		float z = sysVec3.Z;

		float itemWidth = (ImGui.GetColumnWidth() / 3.0f) - 7f;

		float buttonWidth = 5.0f;
		float dragFloatWidth = itemWidth - buttonWidth;

		ImGui.PushStyleVar( ImGuiStyleVar.ItemInnerSpacing, new System.Numerics.Vector2( 0, 0 ) );

		{
			ImGui.PushStyleColor( ImGuiCol.Button, Theme.Red );
			ImGui.Button( $"X##{v}", new( buttonWidth, 0 ) );
			ImGui.SameLine();
			ImGui.PopStyleColor();

			ImGui.SetNextItemWidth( dragFloatWidth );
			ImGui.DragFloat( $"##{v}_x", ref x );
			ImGui.SameLine();
		}

		{
			ImGui.PushStyleColor( ImGuiCol.Button, Theme.Green );
			ImGui.Button( $"Y##{v}", new( buttonWidth, 0 ) );
			ImGui.SameLine();
			ImGui.PopStyleColor();

			ImGui.SetNextItemWidth( dragFloatWidth );
			ImGui.DragFloat( $"##{v}_y", ref y );
			ImGui.SameLine();
		}

		{
			ImGui.PushStyleColor( ImGuiCol.Button, Theme.Blue );
			ImGui.Button( $"Z##{v}", new( buttonWidth, 0 ) );
			ImGui.SameLine();
			ImGui.PopStyleColor();

			ImGui.SetNextItemWidth( dragFloatWidth );
			ImGui.DragFloat( $"##{v}_z", ref z );
		}

		ImGui.PopStyleVar();

		var vec3 = new Vector3( x, y, z );
		bool changed = !(sysVec3.X == vec3.X && sysVec3.Y == vec3.Y && sysVec3.Z == vec3.Z);

		if ( changed )
			sysVec3 = vec3;

		return changed;
	}

	public static void DockSpaceOverViewport()
	{
		var viewport = ImGui.GetMainViewport();

		ImGui.SetNextWindowPos( viewport.WorkPos );
		ImGui.SetNextWindowSize( viewport.WorkSize );
		ImGui.SetNextWindowViewport( viewport.ID );

		var flags =
			ImGuiWindowFlags.NoTitleBar |
			ImGuiWindowFlags.NoCollapse |
			ImGuiWindowFlags.NoResize |
			ImGuiWindowFlags.NoMove |
			ImGuiWindowFlags.NoDocking |
			ImGuiWindowFlags.NoBringToFrontOnFocus |
			ImGuiWindowFlags.NoNavFocus |
			ImGuiWindowFlags.NoBackground;

		ImGui.PushStyleVar( ImGuiStyleVar.WindowRounding, 0 );
		ImGui.PushStyleVar( ImGuiStyleVar.WindowBorderSize, 0 );
		ImGui.PushStyleVar( ImGuiStyleVar.WindowPadding, System.Numerics.Vector2.Zero );

		using ( Scope scope = Begin( "DockSpaceViewport_main", flags ) )
		{
			if ( scope.Visible )
			{
				var dockspaceId = ImGui.GetID( "DockSpace" );
				ImGui.DockSpace( dockspaceId, new System.Numerics.Vector2( 0, 0 ),
					ImGuiDockNodeFlags.PassthruCentralNode | ImGuiDockNodeFlags.AutoHideTabBar );
			}
		}

		var io = ImGui.GetIO();
		io.ConfigDockingAlwaysTabBar = true;

		ImGui.PopStyleVar( 3 );
	}

	public static void InspectorTitle( string text, string subtext, ResourceType resourceType )
	{
		const float heightTop = 56;
		const float heightBottom = 24;
		const float iconSize = 64;

		var color = resourceType.Color * 0.75f;

		var windowPos = ImGui.GetWindowContentRegionMin() + ImGui.GetWindowPos();
		var windowWidth = ImGui.GetWindowContentRegionMax().X;

		windowPos -= new System.Numerics.Vector2( 8, 8 );
		windowWidth += 8;

		var min = windowPos;
		var max = windowPos + new System.Numerics.Vector2( windowWidth, heightTop );

		var drawList = ImGui.GetWindowDrawList();

		drawList.AddRectFilled(
			min,
			max,
			ImGui.GetColorU32( color )
		);

		min = windowPos + new System.Numerics.Vector2( 0, heightTop );
		max = max + new System.Numerics.Vector2( 0, heightBottom );

		drawList.AddRectFilled(
			min,
			max,
			ImGui.GetColorU32( color * 1.25f )
		);

		var cursorPos = ImGui.GetCursorPos();
		Image( resourceType.IconLg, new( iconSize ) );

		ImGui.SetCursorPos( cursorPos );
		SetCursorPosXRelative( iconSize );
		SetCursorPosXRelative( 8 );

		using ( Scope scope = Child( $"title##{text}{subtext}", new System.Numerics.Vector2( 0, heightTop - 8f ), true, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoBackground ) )
		{
			if ( scope.Visible )
			{
				TextSubheading( text );

				SetCursorPosYRelative( -8f );
				ImGui.Text( subtext );
			}
		}

		SetCursorPosXRelative( iconSize );
	}

	public static void Title( string text, string subtext, Vector4? _color = null, bool drawSubpanel = false )
	{
		TextSubheading( text );
		TextLight( subtext );

		ImGui.Dummy( new System.Numerics.Vector2( 0, 4 ) );
	}

	public static void SetCursorPosXRelative( float relativePos )
	{
		ImGui.SetCursorPosX( ImGui.GetCursorPosX() + relativePos );
	}

	public static void SetCursorPosYRelative( float relativePos )
	{
		ImGui.SetCursorPosY( ImGui.GetCursorPosY() + relativePos );
	}

	public static void SetCursorPosRelative( System.Numerics.Vector2 relativePos )
	{
		ImGui.SetCursorPos( ImGui.GetCursorPos() + relativePos );
	}

	public static List<string> MenusSubmittedThisFrame { get; } = new();

	public static Scope Menu( string name )
	{
		SetCursorPosXRelative( 4 );
		ImGui.SetNextWindowSize( new System.Numerics.Vector2( 250, -1 ) );
		bool visible = ImGui.BeginMenu( name );

		return new Scope
		{
			Type = ScopeType.Menu,
			Visible = visible,
			CallEnd = visible
		};
	}

	public static bool MenuItem( string icon, string text, bool enabled = false )
	{
		SetCursorPosYRelative( -4 );

		var drawList = ImGui.GetForegroundDrawList();
		var windowPos = ImGui.GetWindowPos();
		var windowSize = ImGui.GetWindowSize();

		var padding = new System.Numerics.Vector2( 8, 8 );

		var size = new System.Numerics.Vector2( windowSize.X - (padding.X + 16), ImGui.CalcTextSize( text ).Y ) + padding;
		bool result = ImGui.InvisibleButton( $"##menu_{text}", size );
		SetCursorPosYRelative( -size.Y );

		var p0 = ImGui.GetCursorPos() + windowPos - new System.Numerics.Vector2( 0, 2 );
		var p1 = p0 + size + new System.Numerics.Vector2( 0, 4 );

		uint col = ImGui.GetColorU32( new System.Numerics.Vector4( 0, 0, 0, 0.1f ) );

		if ( ImGui.IsItemHovered() )
			drawList.AddRectFilled( p0, p1, col );

		SetCursorPosXRelative( padding.X * 0.5f );

		//ImGui.PushFont( Editor.Subheading );
		ImGui.Text( icon );
		ImGui.SameLine();
		//ImGui.PopFont();

		SetCursorPosYRelative( 4 );
		ImGui.Text( text );

		if ( enabled )
		{
			SetCursorPosYRelative( -4 );
			ImGui.SameLine();
			ImGui.SetCursorPosX( 210 );
			ImGui.PushStyleColor( ImGuiCol.Text, new System.Numerics.Vector4( 1, 1, 1, 0.5f ) );

			ImGui.Text( FontAwesome.Check );
			ImGui.PopStyleColor();
		}

		if ( result )
		{
			ImGui.CloseCurrentPopup();
		}

		return result;
	}

	public static bool GradientButton( string text )
	{
		//
		// This is so unbelievably shit, and so unbelievably
		// hacky, that I think I'm going to write my own GUI
		// solution that actually does support gradients
		// properly because I can't deal with this anymore.
		//

		var cursorPos = ImGui.GetCursorPos();
		ImGui.PushStyleColor( ImGuiCol.Button, Vector4.Zero );
		bool res = ImGui.Button( text );
		ImGui.PopStyleColor();

		var min = ImGui.GetItemRectMin();
		var max = ImGui.GetItemRectMax();

		var drawList = ImGui.GetWindowDrawList();

		drawList.AddRectFilledMultiColor(
			min,
			max,
			ImGui.GetColorU32( MathX.GetColor( "#404040" ) ),
			ImGui.GetColorU32( MathX.GetColor( "#404040" ) ),
			ImGui.GetColorU32( MathX.GetColor( "#333333" ) ),
			ImGui.GetColorU32( MathX.GetColor( "#333333" ) )
		);

		drawList.AddRect(
			min - new System.Numerics.Vector2( 2 ),
			max + new System.Numerics.Vector2( 2 ),
			ImGui.GetColorU32( ImGuiCol.WindowBg ),
			6,
			ImDrawFlags.None,
			3 // rounding - 1px
		);

		drawList.AddRect(
			min - new System.Numerics.Vector2( 0 ),
			max + new System.Numerics.Vector2( 1 ),
			ImGui.GetColorU32( Theme.Gray ),
			4,
			ImDrawFlags.None,
			1
		);

		ImGui.SetCursorPos( cursorPos );
		ImGui.PushStyleColor( ImGuiCol.Button, Vector4.Zero );
		ImGui.Button( text );
		ImGui.PopStyleColor();

		return res;
	}
}
