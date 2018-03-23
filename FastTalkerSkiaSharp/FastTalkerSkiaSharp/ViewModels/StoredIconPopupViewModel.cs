using System;
using System.Collections.Generic;
using FastTalkerSkiaSharp.Helpers;
using FastTalkerSkiaSharp.Models;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace FastTalkerSkiaSharp.ViewModels
{
    public class StoredIconPopupViewModel : PopupUpViewModel
    {
        public event Action<ArgsSelectedIcon> IconSelected = delegate { };

        private ObservableCollection<DisplayImageRowModel> _rows;
        public ObservableCollection<DisplayImageRowModel> Rows 
        { 
            get 
            {
                return _rows;
            }
            set
            {
                _rows = value;
                OnPropertyChanged("Rows");
            }
        }

        public List<SkiaSharp.Elements.Element> ItemsMatching { get; set; }

        public string FolderWithIcons { get; set; }

        public StoredIconPopupViewModel() { }

		public void UnloadInformation()
		{
			Rows.Clear();

			ItemsMatching.Clear();
		}
        
        public void LoadInformationAsync(StackLayout coreLayout)
        {
			System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "LoadInformationAsync(StackLayout coreLayout)");

            if (Rows == null)
            {
                Rows = new ObservableCollection<DisplayImageRowModel>();
            }
            else
            {
                Rows.Clear();
            }

            int i = 0, j = 0;

            ImageSource source1, source2, source3;
            string tempName1, tempName2, tempName3;

            for (i = 0; (j + i) < ItemsMatching.Count;)
            {
                tempName1 = tempName2 = tempName3 = null;
                source1 = source2 = source3 = "";

                System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "ResourceName: " + ItemsMatching[i].ImageInformation);
                System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Width: " + ItemsMatching[i].ImageInformation);

                for (j = 0; j < 3 && (j + i) < ItemsMatching.Count; j++)
                {
                    if (j == 0)
                    {
                        tempName1 = ItemsMatching[j + i].Text;
                        source1 = ImageSource.FromResource(ItemsMatching[j + i].ImageInformation);
                    }
                    else if (j == 1)
                    {
                        tempName2 = ItemsMatching[j + i].Text;
                        source2 = ImageSource.FromResource(ItemsMatching[j + i].ImageInformation);
                    }
                    else if (j == 2)
                    {
                        tempName3 = ItemsMatching[j + i].Text;
                        source3 = ImageSource.FromResource(ItemsMatching[j + i].ImageInformation);
                    }
                }

                j = 0;

                Rows.Add(CreateModel(tempName1, source1, tempName2, source2, tempName3, source3, coreLayout.Width / 4));

                i += 3;
            }
        }

        /// <summary>
        /// Creates a model for collection based on existing references
        /// </summary>
        /// <returns>The model.</returns>
        /// <param name="name1">Name1.</param>
        /// <param name="res1">Res1.</param>
        /// <param name="name2">Name2.</param>
        /// <param name="res2">Res2.</param>
        /// <param name="name3">Name3.</param>
        /// <param name="res3">Res3.</param>
        /// <param name="recommendedWidth">Recommended width.</param>
        private DisplayImageRowModel CreateModel(string name1, ImageSource res1,
                                         string name2, ImageSource res2,
                                         string name3, ImageSource res3,
                                         double recommendedWidth)
        {
            return new DisplayImageRowModel()
            {
                Name1 = name1,
                Image1 = res1,

                Name2 = name2,
                Image2 = res2,

                Name3 = name3,
                Image3 = res3,

                WidthRequest = recommendedWidth,

                TappedCommand = new Command(TappedCommand)
            };
        }

        /// <summary>
        /// Icon in listview was clicked, add to field and close
        /// </summary>
        /// <param name="obj">Object.</param>
        private void TappedCommand(object obj)
        {
            string iconToReintroduce = obj as string;

            if (string.IsNullOrWhiteSpace(iconToReintroduce)) return;

            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Tapped Command: " + obj as string);

            IconSelected(new ArgsSelectedIcon
            {
                Name = iconToReintroduce,
                ImageSource = null
            });

            OnClose(null, null);
        }

        /// <summary>
        /// Call to close
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private async void OnClose(object sender, EventArgs e)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            await PopupNavigation.PopAsync();
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
