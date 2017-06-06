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
   /// Utility for current rate fetching
   /// </summary>
   public class Rates
   {
      public struct Rate
      { 
         private enum EHtmlCurrencyIndex
         {
            Timestamp = 9,
            BuyUSD = 11,
            SellUSD = 13,
            BuyRUR = 19,
            SellRUR = 21
         }

         public void setFromHtml( HtmlNode node )
         {
            ;
            HtmlNodeCollection children = node.ChildNodes;
            timestamp = children[(int)EHtmlCurrencyIndex.Timestamp].InnerText;
            buyUSD = Convert.ToDouble(children[(int)EHtmlCurrencyIndex.BuyUSD].InnerText);
            sellUSD = Convert.ToDouble(children[(int)EHtmlCurrencyIndex.SellUSD].InnerText);
            buyRUR = Convert.ToDouble(children[(int)EHtmlCurrencyIndex.BuyRUR].InnerText);
            sellRUR = Convert.ToDouble(children[(int)EHtmlCurrencyIndex.SellRUR].InnerText);
         }

         public string timestamp;
         public double buyUSD;
         public double sellUSD;
         public double buyRUR;
         public double sellRUR;
      }

      private const string baseURL = @"http://rate.am/";
      private const string hsbcPostfix = @"am/bank/hsbc-bank-armenia/";

      public Rates(DateTime? date)
      {
         HtmlWeb web = new HtmlWeb();
         hsbcId = web.Load(baseURL + hsbcPostfix).DocumentNode.SelectSingleNode("//option[@selected='selected']").Attributes["value"].Value;

         fetch(date);
      }

      public void reload(DateTime? date)
      {
         fetch(date);
      }

      private string generateDateUrlPostfix(DateTime? date)
      {
         if (date.Equals(null))
            return "";

         const string datePostfix = "am/armenian-dram-exchange-rates/banks/cash/";
         DateTime current = (DateTime)date;
         return datePostfix + current.ToString("yyyy/MM/dd") + "/18-30";
      }

      private void fetch(DateTime? date)
      {
         HtmlWeb web = new HtmlWeb();
         HtmlNode ratesInfo = web.Load(baseURL + generateDateUrlPostfix(date)).DocumentNode.SelectSingleNode("//tr[@id='" + hsbcId + "']");

         rate.setFromHtml(ratesInfo);
      }

      public Rate getRates() { return rate; }

      private string hsbcId;
      private Rate rate;
   }

   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      public MainWindow()
      {
         InitializeComponent();
         hsbcRates = new Rates(CalendarScreen.SelectedDate);
      }

      private void Button_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
      {
         StatusBar.Text = "Updating...";
      }

      private void Button_Click(object sender, RoutedEventArgs e)
      {
         hsbcRates.reload(CalendarScreen.SelectedDate);
         DataScreen.Text = hsbcRates.getRates().timestamp +
            "\nBuy USD: " + string.Format("{0:F2}", hsbcRates.getRates().buyUSD) +
            "\nSell USD: " + string.Format("{0:F2}", hsbcRates.getRates().sellUSD) +
            "\nBuy RUR: " + string.Format("{0:F2}", hsbcRates.getRates().buyRUR) +
            "\nSell RUR: " + string.Format("{0:F2}", hsbcRates.getRates().sellRUR);

         StatusBar.Text = "";
      }

      private Rates hsbcRates;
   }
}
