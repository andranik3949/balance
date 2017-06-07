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

      public Rates()
      {
         Init();
      }

      public Rates(DateTime date)
      {
         Init();
         fetch(date);
      }

      private void Init()
      {
         HtmlWeb web = new HtmlWeb();
         hsbcId = web.Load(baseURL + hsbcPostfix).DocumentNode.SelectSingleNode("//option[@selected='selected']").Attributes["value"].Value;
      }

      public void reload(DateTime date)
      {
         fetch(date);
      }

      private void fetch(DateTime date)
      {
         HtmlWeb web = new HtmlWeb();
         HtmlNode ratesInfo = web.Load(baseURL + generateDateUrlPostfix(date)).DocumentNode.SelectSingleNode("//tr[@id='" + hsbcId + "']");

         rate.setFromHtml(ratesInfo);
      }

      private string generateDateUrlPostfix(DateTime date)
      {
         DateTime now = DateTime.Now;
         if (now - date < TimeSpan.FromMinutes(15) )
            return "";

         const string datePostfix = "am/armenian-dram-exchange-rates/banks/cash/";
         return datePostfix + date.ToString("yyyy/MM/dd/HH-mm");
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
         hsbcRates = new Rates();

         StatusBar.Text = "Ready";
      }

      private void Button_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
      {
         StatusBar.Text = "Loading rates...";
      }

      private void Button_Click(object sender, RoutedEventArgs e)
      {
         if (CalendarScreen.SelectedDate == null)
         {
            StatusBar.Text = "Error: No date selected";
            return;
         }

         DateTime requestedDate = (DateTime)CalendarScreen.SelectedDate;
         hsbcRates.reload(requestedDate.AddHours(7 + ((int)TimeSlider.Value / 4) - requestedDate.Hour).AddMinutes(15 * ((int)TimeSlider.Value % 4) - requestedDate.Minute));
         DisplayRates();

         StatusBar.Text = "Ready";
      }

      private void DisplayRates()
      {
         DateField.Text = hsbcRates.getRates().timestamp;
         BuyUSD.Text = string.Format("{0:F2}", hsbcRates.getRates().buyUSD);
         SellUSD.Text = string.Format("{0:F2}", hsbcRates.getRates().sellUSD);
         BuyRUR.Text = string.Format("{0:F2}", hsbcRates.getRates().buyRUR);
         SellRUR.Text = string.Format("{0:F2}", hsbcRates.getRates().sellRUR);
      }

      private Rates hsbcRates;

      private void CalendarScreen_GotMouseCapture(object sender, MouseEventArgs e)
      {
         Mouse.Capture(null);
      }

      private void CavasScreen_MouseDown(object sender, MouseButtonEventArgs e)
      {
         canvasStart = e.GetPosition((Canvas)sender);
      }

      private void CavasScreen_MouseUp(object sender, MouseButtonEventArgs e)
      {
         Canvas c = (Canvas)sender;

         Line l = new Line();
         l.StrokeThickness = 1;
         l.Stroke = System.Windows.Media.Brushes.Black;
         l.X1 = canvasStart.X;
         l.Y1 = canvasStart.Y;
         l.X2 = e.GetPosition(c).X;
         l.Y2 = e.GetPosition(c).Y;
         (c).Children.Add(l);
      }

      private Point canvasStart;

      private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
      {
         int selection = (int)((Slider)sender).Value;
         SelectedTime.Text = string.Format("{0:D2}:{1:D2}", 7 + (selection / 4), 15 * (selection % 4));
      }
   }
}
