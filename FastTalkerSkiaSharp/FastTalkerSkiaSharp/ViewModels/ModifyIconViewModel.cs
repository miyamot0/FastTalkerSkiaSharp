/* 
    The MIT License

    Copyright February 8, 2016 Shawn Gilroy. http://www.smallnstats.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
*/

using FastTalkerSkiaSharp.Controls;
using System.Linq;

namespace FastTalkerSkiaSharp.ViewModels
{
    public class ModifyIconViewModel : PopupUpViewModel
    {
        Icon currentIcon;
        FieldControl controller;

        public System.Windows.Input.ICommand ButtonResetSize { get; set; }
        public System.Windows.Input.ICommand ButtonIncreaseSize { get; set; }
        public System.Windows.Input.ICommand ButtonIncreaseSize2 { get; set; }
        public System.Windows.Input.ICommand ButtonDecreaseSize { get; set; }
        public System.Windows.Input.ICommand ButtonDecreaseSize2 { get; set; }
        public System.Windows.Input.ICommand ButtonEditText { get; set; }
        public System.Windows.Input.ICommand ButtonPinning { get; set; }
        public System.Windows.Input.ICommand ButtonDelete { get; set; }

        public ModifyIconViewModel(Icon _currentIcon, FieldControl _controller)
        {
            currentIcon = _currentIcon;
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
            int index = controller.Icons.IndexOf(currentIcon);

            Icon item = await App.ImageBuilderInstance.AmendIconImage(currentIcon, feedbackOption);

            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Index: " + index);

            if (item == null || index == -1)
            {
                System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "was null or unrefernced");
            }
            else
            {
                if (currentIcon.Tag == IconRoles.GetRoleInt(IconRoles.Role.Folder))
                {
                    string oldFolderTitle = currentIcon.Text;

                    if (controller.Icons.Where(elem => elem.IsStoredInAFolder && elem.StoredFolderTag == oldFolderTitle).Any())
                    {
                        // Modify dated tags
                        for (int i = 0; i < controller.Icons.Count; i++)
                        {
                            controller.Icons[i].StoredFolderTag = (controller.Icons[i].IsStoredInAFolder &&
                                                                      controller.Icons[i].Tag == IconRoles.GetRoleInt(IconRoles.Role.Communication) &&
                                                                      controller.Icons[i].StoredFolderTag == oldFolderTitle) ? item.Text : controller.Icons[i].StoredFolderTag;
                        }
                    }
                }

                controller.Icons[index] = item;

                controller.Invalidate();

                controller.PromptResave();

                currentIcon = controller.Icons[index];
            }
        }

        /// <summary>
        /// Resets the size of button.
        /// </summary>
        void ResetSizeOfButton()
        {
            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Reset Size");

            UpdateIconInController(Constants.LanguageSettings.EditResetSize);
        }

        /// <summary>
        /// Increase size of icon
        /// </summary>
        void IncreaseSizeOfIcon()
        {
            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Increase Size");

            UpdateIconInController(Constants.LanguageSettings.EditGrow);
        }

        /// <summary>
        /// Increase size of icon a lot
        /// </summary>
        void IncreaseSizeOfIconLarge()
        {
            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Increase Size Large");

            UpdateIconInController(Constants.LanguageSettings.EditGrow2);
        }

        /// <summary>
        /// Decrease size of icon
        /// </summary>
        void DecreaseSizeOfIcon()
        {
            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Decrease Size");

            UpdateIconInController(Constants.LanguageSettings.EditShrink);
        }

        /// <summary>
        /// Decrease size of an icon
        /// </summary>
        void DecreaseSizeOfIconLarge()
        {
            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Decrease Size Large");

            UpdateIconInController(Constants.LanguageSettings.EditShrink2);
        }

        /// <summary>
        /// Edits the icon text.
        /// </summary>
        void EditIconText()
        {
            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Edit Text");

            UpdateIconInController(Constants.LanguageSettings.EditText);
        }

        /// <summary>
        /// Pins the icon.
        /// </summary>
        void PinIcon()
        {
            System.Diagnostics.Debug.WriteLineIf(App.OutputVerbose, "Pin Icon");

            UpdateIconInController(Constants.LanguageSettings.EditPinning);
        }

        /// <summary>
        /// Confirm deletion
        /// </summary>
        void DeleteIcon()
        {
            if (currentIcon == null) return;

            switch (currentIcon.Tag)
            {
                case (int)IconRoles.Role.Folder:
                    App.UserInputInstance.ConfirmDeleteFolder(currentIcon);

                    break;

                case (int)IconRoles.Role.Communication:
                    App.UserInputInstance.ConfirmRemoveIcon(currentIcon);

                    break;
            }
        }

        /// <summary>
        /// Update current icon
        /// </summary>
        /// <param name="_currentIcon">Current element.</param>
        public void UpdateIcon(Icon _currentIcon)
        {
            currentIcon = _currentIcon;
        }
    }
}
