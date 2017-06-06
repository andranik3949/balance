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

         web = new HtmlWeb();
         var hsbcInfo = web.Load(baseURL + hsbcPostfix);

         hsbcID = hsbcInfo.DocumentNode.SelectSingleNode("//option[@selected='selected']").Attributes["value"].Value;
      }
      
      private void Button_Click(object sender, RoutedEventArgs e)
      {
         TextBlock screen = FindName("Varung") as TextBlock;
         
         var ratesInfo = web.Load(baseURL).DocumentNode.SelectSingleNode("//tr[@id='" + hsbcID + "']");
         var date = ratesInfo.ChildNodes[9].InnerText;
         var dollarRate = ratesInfo.ChildNodes[11].InnerText;

         screen.Text = date + "\n$1 = " + dollarRate + "դր";
      }

      private HtmlWeb web;
      private const string baseURL     = @"http://rate.am/";
      private const string hsbcPostfix = @"am/bank/hsbc-bank-armenia/";
      private string hsbcID;
   }
}
