﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

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
    }
}
