using System;
using ConsoleApp.UI.Commands;
using SadRogue.Primitives;

namespace ConsoleApp.UI.Controls
{
    public class MessageBox : Dialog
    {
        public MessageBox()
        {
        }

        public static MessageBox Create()
        {
            var messageBox = new MessageBox
            {
                Background = Color.DarkGray,
                Foreground = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 44,
                Height = 12
            };

            var checkBox = new CheckBox
            {
                Background = Color.DarkGray,
                Foreground = Color.Black,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Top = 2,
                Left = 4,
                Width = 22,
                Height = 1,
                IsTriState = false,
                State = CheckBoxState.Checked,
                Text = "Check box"
            };

            var entry = new Entry
            {
                Background = Color.DarkBlue,
                Foreground = Color.White,
                Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Top = 4,
                Left = 4,
                Width = 22,
                Height = 1
            };

            var cancelButton = new Button
            {
                IsCancel = true,
                Command = new DelegateCommand(messageBox.DoCancel),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Text = "Cancel",
                Left = 3,
                Width = 11
            };

            var okButton = new Button
            {
                IsDefault = true,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Text = "OK",
                Left = 16,
                Width = 9
            };

            messageBox.Children.Add(checkBox);
            messageBox.Children.Add(entry);
            messageBox.Children.Add(cancelButton);
            messageBox.Children.Add(okButton);

            return messageBox;
        }

        private void DoCancel()
        {
            Dismiss(DialogDismissReason.UserCancel);
        }
    }
}