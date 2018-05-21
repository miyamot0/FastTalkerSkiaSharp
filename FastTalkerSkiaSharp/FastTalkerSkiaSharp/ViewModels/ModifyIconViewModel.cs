/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL 
   was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu
*/

using System.Linq;

namespace FastTalkerSkiaSharp.ViewModels
{
    public class ModifyIconViewModel : PopupUpViewModel
    {
        SkiaSharp.Elements.Element currentElement;
        SkiaSharp.Elements.ElementsController controller;

        public System.Windows.Input.ICommand ButtonResetSize { get; set; }
        public System.Windows.Input.ICommand ButtonIncreaseSize { get; set; }
        public System.Windows.Input.ICommand ButtonIncreaseSize2 { get; set; }
        public System.Windows.Input.ICommand ButtonDecreaseSize { get; set; }
        public System.Windows.Input.ICommand ButtonDecreaseSize2 { get; set; }
        public System.Windows.Input.ICommand ButtonEditText { get; set; }
        public System.Windows.Input.ICommand ButtonPinning { get; set; }
        public System.Windows.Input.ICommand ButtonDelete { get; set; }

        public ModifyIconViewModel(SkiaSharp.Elements.Element _currentElement, SkiaSharp.Elements.ElementsController _controller)
        {
            currentElement = _currentElement;
            controller = _controller;

            ButtonResetSize = new Xamarin.Forms.Command(ResetSizeOfButton);

            ButtonIncreaseSize = new Xamarin.Forms.Command(IncreaseSizeOfIcon);
            ButtonIncreaseSize2 = new Xamarin.Forms.Command(IncreaseSizeOfIconLarge);

            ButtonDecreaseSize = new Xamarin.Forms.Command(DecreaseSizeOfIcon);
            ButtonDecreaseSize2 = new Xamarin.Forms.Command(DecreaseSizeOfIconLarge);

            ButtonEditText = new Xamarin.Forms.Command(EditIconText);
            ButtonPinning = new Xamarin.Forms.Command(PinIcon);
            ButtonDelete = new Xamarin.Forms.Command(DeleteIcon);
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
                if (currentElement.Tag == FastTalkerSkiaSharp.Elements.ElementRoles.GetRoleInt(FastTalkerSkiaSharp.Elements.ElementRoles.Role.Folder))
                {
                    string oldFolderTitle = currentElement.Text;

                    if (controller.Elements.Where(elem => elem.IsStoredInAFolder && elem.StoredFolderTag == oldFolderTitle).Any())
                    {
                        // Modify dated tags
                        for (int i = 0; i < controller.Elements.Count; i++)
                        {
                            controller.Elements[i].StoredFolderTag = (controller.Elements[i].IsStoredInAFolder &&
                                                                      controller.Elements[i].Tag == FastTalkerSkiaSharp.Elements.ElementRoles.GetRoleInt(FastTalkerSkiaSharp.Elements.ElementRoles.Role.Communication) &&
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

            UpdateIconInController(FastTalkerSkiaSharp.Constants.LanguageSettings.EditResetSize);
        }

        /// <summary>
        /// Increase size of icon
        /// </summary>
        void IncreaseSizeOfIcon()
        {
            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Increase Size");

            UpdateIconInController(FastTalkerSkiaSharp.Constants.LanguageSettings.EditGrow);
        }

        /// <summary>
        /// Increase size of icon a lot
        /// </summary>
        void IncreaseSizeOfIconLarge()
        {
            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Increase Size Large");

            UpdateIconInController(FastTalkerSkiaSharp.Constants.LanguageSettings.EditGrow2);
        }

        /// <summary>
        /// Decrease size of icon
        /// </summary>
        void DecreaseSizeOfIcon()
        {
            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Decrease Size");

            UpdateIconInController(FastTalkerSkiaSharp.Constants.LanguageSettings.EditShrink);
        }

        /// <summary>
        /// Decrease size of an icon
        /// </summary>
        void DecreaseSizeOfIconLarge()
        {
            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Decrease Size Large");

            UpdateIconInController(FastTalkerSkiaSharp.Constants.LanguageSettings.EditShrink2);
        }

        /// <summary>
        /// Edits the icon text.
        /// </summary>
        void EditIconText()
        {
            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Edit Text");

            UpdateIconInController(FastTalkerSkiaSharp.Constants.LanguageSettings.EditText);
        }

        /// <summary>
        /// Pins the icon.
        /// </summary>
        void PinIcon()
        {
            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Pin Icon");

            UpdateIconInController(FastTalkerSkiaSharp.Constants.LanguageSettings.EditPinning);
        }
    
        /// <summary>
        /// Confirm deletion
        /// </summary>
        void DeleteIcon()
        {
            if (currentElement == null) return;

            switch (currentElement.Tag)
            {
                case (int)FastTalkerSkiaSharp.Elements.ElementRoles.Role.Folder:
                    App.UserInputInstance.ConfirmDeleteFolder(currentElement);

                    break;

                case (int)FastTalkerSkiaSharp.Elements.ElementRoles.Role.Communication:
                    App.UserInputInstance.ConfirmRemoveIcon(currentElement);

                    break;
            }
        }
    
        /// <summary>
        /// Update current icon
        /// </summary>
        /// <param name="_currentElement">Current element.</param>
		public void UpdateIcon(SkiaSharp.Elements.Element _currentElement)
		{
			currentElement = _currentElement;
		}
	}
}
