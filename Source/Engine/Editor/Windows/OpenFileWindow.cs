//namespace Mocha.Editor;

//public class OpenFileWindow : EditorWindow
//{
//	private List<string> Matches { get; set; }

//	public string Filter { get; set; }
//	public Action<string> OnSelected { get; set; }

//	const int MAX_INPUT_LENGTH = 512;
//	static string searchInput = "";

//	public OpenFileWindow()
//	{
//		Editor.EditorWindows.RemoveAll( x => x is OpenFileWindow );
//		Editor.EditorWindows.Add( this );
//	}

//	private void GetAllMatches()
//	{
//		Matches = new();

//		var filters = Filter.Split( "," );

//		foreach ( var filter in filters )
//		{
//			Matches.AddRange( FileSystem.Game.FindAllFiles( filter.Trim() ).ToList() );
//		}

//		Matches = Matches.Select( FileSystem.Game.GetRelativePath ).Select( x => x.NormalizePath() ).ToList();
//		Matches.Sort();
//	}

//	public void Show()
//	{
//		GetAllMatches();
//		isVisible = true;
//	}

//	public override void Draw()
//	{
//		using ( ImGuiX.Scope windowScope = ImGuiX.Begin( "Select File" ) )
//		{
//			if ( !windowScope.Visible )
//				return;

//			ImGui.SetNextItemWidth( -1 );
//			searchInput = ImGui.InputText( "##search_input", searchInput, MAX_INPUT_LENGTH );

//			using ( ImGuiX.Scope childScope = ImGuiX.Child( "##file_list", -1, -1 ) )
//			{
//				if ( !childScope.Visible )
//					return;

//				using ( ImGuiX.Scope tableScope = ImGuiX.Table( "##file_List_table", 1, 0 )
//				{
//					if ( !tableScope.Visible )
//						return;

//					ImGui.TableSetupStretchColumn( "Name" );

//					foreach ( var item in Matches )
//					{
//						if ( !string.IsNullOrEmpty( searchInput ) && !item.Contains( searchInput ) )
//							continue;

//						ImGui.TableNextRow();
//						ImGui.TableNextColumn();

//						if ( ImGui.Selectable( item ) )
//						{
//							OnSelected?.Invoke( item );
//							Editor.EditorWindows.Remove( this );
//						}
//					}
//				}
//			}
//		}
//	}
//}
