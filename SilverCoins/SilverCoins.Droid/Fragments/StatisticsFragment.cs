using Android.OS;
using Android.Views;
using Android.Widget;
using Com.Syncfusion.Charts;
using Com.Syncfusion.Charts.Enums;
using SilverCoins.BusinessLayer.Managers;
using SilverCoins.BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Fragment = Android.Support.V4.App.Fragment;

namespace SilverCoins.Droid.Fragments
{
    public class StatisticsFragment : Fragment
    {
        private SfChart chart;
        private Spinner spinnerAccounts;
        private List<Account> listOfAccounts = SilverCoinsManager.GetAccounts().ToList();
        private Account account;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            HasOptionsMenu = true;

            listOfAccounts.Insert(0, new Account() { Name = "All accounts", Id = 0 });

            spinnerAccounts = Activity.FindViewById<Spinner>(Resource.Id.spinnerAccounts);
            spinnerAccounts.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(SpinnerTransactionAccount_ItemClick);

            account = listOfAccounts[spinnerAccounts.SelectedItemPosition];

            chart = new SfChart(Activity);

            ShowPieChart();

            return chart;
        }

        private void SpinnerTransactionAccount_ItemClick(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            account = listOfAccounts[spinnerAccounts.SelectedItemPosition];
            switch (chart.Title.Text)
            {
                case "Income & Expenses":
                    ShowPieChart();
                    break;

                case "Cash flow by type":
                    ShowBarChart();
                    break;

                case "Balance":
                    ShowColumnChart();
                    break;

                default:
                    break;
            }
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            inflater.Inflate(Resource.Menu.menu_statistics, menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_piechart:
                    ShowPieChart();
                    return true;

                case Resource.Id.action_linechart:
                    ShowBarChart();
                    return true;

                case Resource.Id.action_barchart:
                    ShowColumnChart();
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        private void ShowPieChart()
        {
            chart.Series.Clear();
            chart.Title.Text = "Income & Expenses";
            chart.Legend.Visibility = Visibility.Visible;

            var dataModel = Statistics.Statistics.PieChartData(account);

            PieSeries pieSeries = new PieSeries()
            {
                DataSource = dataModel,
                DataPointSelectionEnabled = true,
                DataMarkerPosition = CircularSeriesDataMarkerPosition.Outside,
                AnimationEnabled = true,
                ExplodableOnTouch = true,
                SmartLabelsEnabled = true,
                Visibility = Visibility.Visible,
                TooltipEnabled = true,
            };

            chart.Series.Add(pieSeries);
        }

        private void ShowBarChart()
        {
            chart.Series.Clear();
            chart.Title.Text = "Cash flow by type";
            chart.Legend.Visibility = Visibility.Visible;

            CategoryAxis primaryAxis = new CategoryAxis();
            primaryAxis.Title.Text = "Type";
            chart.PrimaryAxis = primaryAxis;

            //Initializing Secondary Axis
            NumericalAxis secondaryAxis = new NumericalAxis();
            secondaryAxis.Title.Text = "Amount";
            chart.SecondaryAxis = secondaryAxis;

            var dataModel = Statistics.Statistics.BarChartData(account);

            BarSeries areaSeries1 = new BarSeries()
            {
                DataSource = dataModel[0],
                TooltipEnabled = true,
                AnimationEnabled = true,
                AnimationDuration = 0.2,
                Label = "Income",
            };

            chart.Series.Add(areaSeries1);

            BarSeries areaSeries2 = new BarSeries()
            {
                DataSource = dataModel[1],
                TooltipEnabled = true,
                AnimationEnabled = true,
                AnimationDuration = 0.2,
                Label = "Expenses",
            };

            chart.Series.Add(areaSeries2);
        }

        private void ShowColumnChart()
        {
            chart.Series.Clear();
            chart.Title.Text = "Balance";
            chart.Legend.Visibility = Visibility.Visible;

            CategoryAxis primaryAxis = new CategoryAxis();
            primaryAxis.Title.Text = "Month";
            chart.PrimaryAxis = primaryAxis;

            //Initializing Secondary Axis
            NumericalAxis secondaryAxis = new NumericalAxis();
            secondaryAxis.Title.Text = "Amount";
            chart.SecondaryAxis = secondaryAxis;

            var dataModel = Statistics.Statistics.ColumnChartData(account);

            if (account.Id == 0)
            {
                int counter = 1;
                foreach (var item in dataModel)
                {
                    chart.Series.Add(new ColumnSeries()
                    {
                        DataSource = item,
                        AnimationEnabled = true,
                        AnimationDuration = 0.2,
                        DataPointSelectionEnabled = true,
                        Label = listOfAccounts[counter].Name,
                        StrokeWidth = 7,
                        TooltipEnabled = true,
                    });

                    counter += 1;
                }
            }
            else
            {
                chart.Series.Add(new ColumnSeries()
                {
                    DataSource = dataModel.FirstOrDefault(),
                    AnimationEnabled = true,
                    AnimationDuration = 0.2,
                    DataPointSelectionEnabled = true,
                    Label = account.Name,
                    StrokeWidth = 7,
                    TooltipEnabled = true,
                });
            }
        }
    }
}