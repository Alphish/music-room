using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using Alphicsh.MusicRoom.Model;
using Alphicsh.Ston;

namespace Alphicsh.MusicRoom
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // registers the parameter value change while keeping text box focused on
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textBox = sender as TextBox;

                var expression = BindingOperations.GetBindingExpression(textBox, TextBox.TextProperty);
                expression.UpdateSource();

                // this is overly specific to numeric text boxes, like the ones in the loop stream provider
                // I should get rid of it as soon as I make proper watermarked textboxes
                // so that the text for defaults is *always* an empty string
                // and if the text is empty string, the watermark displays
                var binding = BindingOperations.GetBinding(textBox, TextBox.TextProperty);
                if (binding.Converter is Converters.NumericWithDefaultConverter)
                {
                    if (binding.ConverterParameter.Equals(textBox.Text))
                        textBox.Text = "";
                }
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Playlist playlist = null;

            if (!e.Args.Any())
                playlist = new Playlist();
            else
            {
                try
                {
                    playlist = Playlist.Load(e.Args[0]);
                }
                catch (IOException ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException)
                {
                    MessageBox.Show($"Could not find playlist at the following path:\n{e.Args[0]}\n\nThe player will open with default settings.");
                }
                catch (StonException)
                {
                    MessageBox.Show("The playlist provided is invalid.\n\nThe player will open with default settings.");
                }
                finally
                {
                    playlist = playlist ?? new Playlist();
                }
            }

            var window = new MusicRoom(playlist);
            window.Show();
        }
    }
}
