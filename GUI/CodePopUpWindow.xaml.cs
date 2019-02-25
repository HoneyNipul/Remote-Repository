//////////////////////////////////////////////////////////////////////
// CodePopUpWindow.xaml.cs - GUI for code pop up                     //
// ver 1.0                                                           //
// Honey Shah, Ms Computer Science                                   //
// Source :Jim Fawcett,                                              //
// CSE687-OnLine Object Oriented Design, Spring 2018                 //
///////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package provides a WPF-based GUI for Project3HelpWPF demo.  It's 
 * responsibilities are to:
 * -Provides the display for the file the content of the file
  
 * Required Files:
 * ---------------
 * CodePopUpWindow.xaml, CodePopUpWindow.xaml.cs
 * Translater.dll
 * 
 * Maintenance History:
 * --------------------
 *
 * ver 1.0:   12 April 2018
 *  - First release
 *  
 */

// Translater has to be statically linked with CommLibWrapper
// - loader can't find Translater.dll dependent CommLibWrapper.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for CodePopUpWindow.xaml
    /// </summary>
    public partial class CodePopUpWindow : Window
    {
        private static double leftOffset = 500.0;
        private static double topOffset = -20.0;

        public CodePopUpWindow()
        {
            InitializeComponent();
            double Left = Application.Current.MainWindow.Left;
            double Top = Application.Current.MainWindow.Top;
            this.Left = Left + leftOffset;
            this.Top = Top + topOffset;
            this.Width = 600.0;
            this.Height = 800.0;
            leftOffset += 20.0;
            topOffset += 20.0;
            if (leftOffset > 700.0)
                leftOffset = 500.0;
            if (topOffset > 180.0)
                topOffset = -20.0;
        }

        //<----------------- Button event for Exiting PopUP---------->
        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
