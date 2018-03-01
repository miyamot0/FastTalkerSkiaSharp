using System;
using System.Windows.Input;
using Xamarin.Forms;
using FastTalkerSkiaSharp.Constants;
using System.Linq;

namespace FastTalkerSkiaSharp.ViewModels
{
    public class ModifyIconViewModel : PopupUpViewModel
    {
        SkiaSharp.Elements.Element currentElement;
        SkiaSharp.Elements.ElementsController controller;

        public ICommand ButtonResetSize { get; set; }
        public ICommand ButtonIncreaseSize { get; set; }
        public ICommand ButtonIncreaseSize2 { get; set; }
        public ICommand ButtonDecreaseSize { get; set; }
        public ICommand ButtonDecreaseSize2 { get; set; }
        public ICommand ButtonEditText { get; set; }
        public ICommand ButtonPinning { get; set; }

        public ModifyIconViewModel(SkiaSharp.Elements.Element _currentElement, SkiaSharp.Elements.ElementsController _controller)
        {
            currentElement = _currentElement;
            controller = _controller;

            ButtonResetSize = new Command(ResetSizeOfButton);

            ButtonIncreaseSize = new Command(IncreaseSizeOfIcon);
            ButtonIncreaseSize2 = new Command(IncreaseSizeOfIconLarge);

            ButtonDecreaseSize = new Command(DecreaseSizeOfIcon);
            ButtonDecreaseSize2 = new Command(DecreaseSizeOfIconLarge);

            ButtonEditText = new Command(EditIconText);
            ButtonPinning = new Command(PinIcon);
        }

        /// <summary>
        /// Update icon
        /// </summary>
        /// <param name="feedbackOption">Feedback option.</param>
        async void UpdateIconInController(string feedbackOption)
        {
            int index = controller.Elements.IndexOf(currentElement);

            SkiaSharp.Elements.Element item = await App.ImageBuilderInstance.AmendIconImage(currentElement, feedbackOption);

            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Index: " + index);

            if (item == null || index == -1)
            {
                System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "was null or unrefernced");
            }
            else
            {
                if (currentElement.Tag == (int)SkiaSharp.Elements.CanvasView.Role.Folder)
                {
                    string oldFolderTitle = currentElement.Text;

                    if (controller.Elements.Where(elem => elem.IsStoredInAFolder && elem.StoredFolderTag == oldFolderTitle).Any())
                    {
                        // Modify dated tags
                        for (int i = 0; i < controller.Elements.Count; i++)
                        {
                            controller.Elements[i].StoredFolderTag = (controller.Elements[i].IsStoredInAFolder &&
                                                                      controller.Elements[i].Tag == (int)SkiaSharp.Elements.CanvasView.Role.Communication &&
                                                                      controller.Elements[i].StoredFolderTag == oldFolderTitle) ? item.Text : controller.Elements[i].StoredFolderTag;
                        }
                    }
                }

                controller.Elements[index] = item;

                controller.Invalidate();

                controller.PromptResave();

                currentElement = controller.Elements[index];
            }
        }

        /// <summary>
        /// Resets the size of button.
        /// </summary>
        void ResetSizeOfButton()
        {
            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose,"Reset Size");

            UpdateIconInController(LanguageSettings.EditResetSize);
        }

        /// <summary>
        /// Increase size of icon
        /// </summary>
        void IncreaseSizeOfIcon()
        {
            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Increase Size");

            UpdateIconInController(LanguageSettings.EditGrow);
        }

        /// <summary>
        /// Increase size of icon a lot
        /// </summary>
        void IncreaseSizeOfIconLarge()
        {
            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Increase Size Large");

            UpdateIconInController(LanguageSettings.EditGrow2);
        }

        /// <summary>
        /// Decrease size of icon
        /// </summary>
        void DecreaseSizeOfIcon()
        {
            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Decrease Size");

            UpdateIconInController(LanguageSettings.EditShrink);
        }

        /// <summary>
        /// Decrease size of an icon
        /// </summary>
        void DecreaseSizeOfIconLarge()
        {
            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Decrease Size Large");

            UpdateIconInController(LanguageSettings.EditShrink2);
        }

        /// <summary>
        /// Edits the icon text.
        /// </summary>
        void EditIconText()
        {
            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Edit Text");

            UpdateIconInController(LanguageSettings.EditText);
        }

        /// <summary>
        /// Pins the icon.
        /// </summary>
        void PinIcon()
        {
            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Pin Icon");

            UpdateIconInController(LanguageSettings.EditPinning);
        }
    }
}
