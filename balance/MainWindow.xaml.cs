using HtmlAgilityPack;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Balance
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      public MainWindow()
      {
         InitializeComponent();
      }
      
      private void Button_Click(object sender, RoutedEventArgs e)
      {
         TextBlock screen = FindName("Varung") as TextBlock;
         
         var html = @"http://rate.am/";
         HtmlWeb web = new HtmlWeb();
         var htmlDoc = web.Load(html);
         try
         {
            var node = htmlDoc.DocumentNode.SelectSingleNode("//img[@alt='HSBC Bank Armenia']");
            
            screen.Text = "Node Name: " + node.NextSibling.OuterHtml;
         }
         catch( Exception ex )
         {
            screen.Text = ex.Source + " " + ex.Message;
         }
      }
   }
}
