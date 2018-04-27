using System;
using System.IO;
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
			int rotation1, rotation2, rotation3;

			rotation1 = rotation2 = rotation3 = 0;

            for (i = 0; (j + i) < ItemsMatching.Count;)
            {
                tempName1 = tempName2 = tempName3 = null;
                source1 = source2 = source3 = "";

                System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "ResourceName: " + ItemsMatching[i].ImageInformation);
                System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Width: " + ItemsMatching[i].ImageInformation);

                for (j = 0; j < 3 && (j + i) < ItemsMatching.Count; j++)
                {
					System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "ResourceName: " + ItemsMatching[j + i].Text +
					                                     " Local: " + ItemsMatching[j + i].LocalImage);

					if (j == 0)
					{
						tempName1 = ItemsMatching[j + i].Text;

						if (ItemsMatching[j + i].LocalImage)
						{
							source1 = ImageSource.FromResource(ItemsMatching[j + i].ImageInformation);
							rotation1 = 0;
						}
						else
						{
							byte[] data = System.Convert.FromBase64String(ItemsMatching[j + i].ImageInformation);
							source1 = ImageSource.FromStream(() => new MemoryStream(data));
                            rotation1 = 180;
						}
                    }
                    else if (j == 1)
                    {
                        tempName2 = ItemsMatching[j + i].Text;

						if (ItemsMatching[j + i].LocalImage)
                        {
							source2 = ImageSource.FromResource(ItemsMatching[j + i].ImageInformation);
                            rotation2 = 0;
                        }
                        else
                        {
							byte[] data = System.Convert.FromBase64String(ItemsMatching[j + i].ImageInformation);
							source2 = ImageSource.FromStream(() => new MemoryStream(data));
                            rotation2 = 180;
                        }
                    }
                    else if (j == 2)
                    {
                        tempName3 = ItemsMatching[j + i].Text;

						if (ItemsMatching[j + i].LocalImage)
                        {
							source3 = ImageSource.FromResource(ItemsMatching[j + i].ImageInformation);
                            rotation3 = 0;
                        }
                        else
                        {
                            byte[] data = System.Convert.FromBase64String(ItemsMatching[j + i].ImageInformation);
							source3 = ImageSource.FromStream(() => new MemoryStream(data));
                            rotation3 = 180;
                        }
                    }
                }

                j = 0;

				Rows.Add(CreateModel(tempName1, source1, rotation1,
				                     tempName2, source2, rotation2,
				                     tempName3, source3, rotation3,
				                     coreLayout.Width / 4));

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
        private DisplayImageRowModel CreateModel(string name1, ImageSource res1, int rot1,
                                         string name2, ImageSource res2, int rot2,
                                         string name3, ImageSource res3, int rot3,
                                         double recommendedWidth)
        {
            return new DisplayImageRowModel()
            {
                Name1 = name1,
                Image1 = res1,
				Rotation1 = rot1,

                Name2 = name2,
                Image2 = res2,
				Rotation2 = rot2,

                Name3 = name3,
                Image3 = res3,
				Rotation3 = rot3,

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
