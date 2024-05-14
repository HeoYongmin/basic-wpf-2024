using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ex12_Busan_ExhibitionList.Models;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using static Azure.Core.HttpHeader;


namespace ex12_Busan_ExhibitionList
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        bool isFavorite = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TxtSearch.Focus();
        }

        private async void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string openApiUri = "https://apis.data.go.kr/6260000/BusanCultureExhibitDetailService/getBusanCultureExhibitDetail?serviceKey=AKkxGajLCIxIsoqRx3dcgmce8WdKEhnXBTy5pBLdMPMXI%2F1PiZlSpRk8eBiFGphchoX4N5PXwSaMAbiy8tA8ww%3D%3D&pageNo=1&numOfRows=10&resultType=json";
            string result = string.Empty;

            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            try
            {
                req = WebRequest.Create(openApiUri);
                res = await req.GetResponseAsync();
                reader = new StreamReader(res.GetResponseStream());
                result = await reader.ReadToEndAsync();

               // await this.ShowMessageAsync("결과", result);

               
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"OpenAPI 조회오류 {ex.Message}");
            }

            var jsonResult = JObject.Parse(result);
            var status = Convert.ToInt32(jsonResult["status"]);

            var code = Convert.ToString(jsonResult["getBusanCultureExhibitDetail"]["header"]["code"]);

            try
            {
                if (code == "00")
                {
                    var data = jsonResult["getBusanCultureExhibitDetail"]["item"];
                    var jsonArray = data as JArray;

                    var exhibit = new List<Exhibit>();
                    foreach (var bced in jsonArray)
                    {
                        exhibit.Add(new Exhibit()
                        {
                            Id = 0,
                            Res_no = Convert.ToInt32(bced["res_no"]),
                            Title = Convert.ToString(bced["title"]),
                            Op_st_dt = Convert.ToString(bced["op_st_dt"]),
                            Op_ed_dt = Convert.ToString(bced["op_ed_dt"]),
                            Op_at = Convert.ToString(bced["op_at"]),
                            Place_Id = Convert.ToString(bced["place_id"]),
                            Place_nm = Convert.ToString(bced["place_nm"]),
                            Showtime = Convert.ToString(bced["showtime"]),
                            Price = Convert.ToString(bced["price"]),
                            Crew = Convert.ToString(bced["crew"]),
                            Dabom_url = Convert.ToString(bced["dabom_url"])
                        });
                    }

                    this.DataContext = exhibit;
                    StsResult.Content = $"OpenAPI {exhibit.Count}건 조회완료!";
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"JSON 처리 오류 {ex.Message}");
            }
        }
        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnSearch_Click(sender, e);
            }
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.SelectedItems.Count == 0)
            {
                await this.ShowMessageAsync("저장오류", "선택된 전시회가 없습니다. 전시회를 선택한 후에 저장하세요");
                return;
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
                {
                    conn.Open();

                    var insRes = 0;
                    foreach (Exhibit bced in GrdResult.SelectedItems)
                    {
                        SqlCommand cmd = new SqlCommand(Models.Exhibit.INSERT_QUERY, conn);
                        //cmd.Parameters.AddWithValue("@Id", bced.Id);
                        cmd.Parameters.AddWithValue("@Res_no", bced.Res_no);
                        cmd.Parameters.AddWithValue("@Title", bced.Title);
                        cmd.Parameters.AddWithValue("@Op_st_dt", bced.Op_st_dt);
                        cmd.Parameters.AddWithValue("@Op_ed_dt", bced.Op_ed_dt);
                        cmd.Parameters.AddWithValue("@Op_at", bced.Op_at);
                        cmd.Parameters.AddWithValue("@Place_Id", bced.Place_Id);
                        cmd.Parameters.AddWithValue("@Place_nm", bced.Place_nm);
                        cmd.Parameters.AddWithValue("@Showtime", bced.Showtime);
                        cmd.Parameters.AddWithValue("@Price", bced.Price);
                        cmd.Parameters.AddWithValue("@Crew", bced.Crew);
                        cmd.Parameters.AddWithValue("@Dabom_url", bced.Dabom_url);

                        insRes += cmd.ExecuteNonQuery();

                    }
                    if (insRes > 0)
                    {
                        await this.ShowMessageAsync("저장", "선택된 전시회가 DB에 저장되었습니다!");
                        StsResult.Content = $"DB저장된 전시회 수 {insRes}건";
                    }

                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("저장오류", $"저장오류{ex.Message}");
            }
        }


        private async void BtnFavSearch_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = null;
            TxtSearch.Text = string.Empty;

            List<Exhibit> favExhibit = new List<Exhibit>();

            try
            {
                using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
                {
                    conn.Open();

                    var cmd = new SqlCommand(Models.Exhibit.SELECT_QUERY, conn);
                    var adapter = new SqlDataAdapter(cmd);
                    var dSet = new DataSet();
                    adapter.Fill(dSet, "Exhibit");

                    foreach (DataRow row in dSet.Tables["Exhibit"].Rows)
                    {
                        var exhibit = new Exhibit()
                        {
                            Id = 0,
                            Res_no = Convert.ToInt32(row["res_no"]),
                            Title = Convert.ToString(row["title"]),
                            Op_st_dt = Convert.ToString(row["op_st_dt"]),
                            Op_ed_dt = Convert.ToString(row["op_ed_dt"]),
                            Op_at = Convert.ToString(row["op_at"]),
                            Place_Id = Convert.ToString(row["place_id"]),
                            Place_nm = Convert.ToString(row["place_nm"]),
                            Showtime = Convert.ToString(row["showtime"]),
                            Price = Convert.ToString(row["price"]),
                            Crew = Convert.ToString(row["crew"]),
                            Dabom_url = Convert.ToString(row["dabom_url"])
                        };

                        favExhibit.Add(exhibit);
                    }
                    this.DataContext = favExhibit;
                    isFavorite = true;
                    StsResult.Content = $"즐겨찾기 {favExhibit.Count}건 조회완료";
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"즐겨찾기 오류 {ex.Message}");
            }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (isFavorite == false)
            {
                await this.ShowMessageAsync("삭제", "즐겨찾기한 전시회가 아닙니다.");
                return;
            }

            if (GrdResult.SelectedItems.Count == 0)
            {
                await this.ShowMessageAsync("삭제", "삭제할 전시회를 선택하세요.");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
                {
                    conn.Open();

                    var delRes = 0;

                    foreach (Exhibit bced in GrdResult.SelectedItems)
                    {
                        SqlCommand cmd = new SqlCommand(Models.Exhibit.DELETE_QUERY, conn);
                        cmd.Parameters.AddWithValue("@Res_no", bced.Res_no);

                        delRes += cmd.ExecuteNonQuery();
                    }
                    if (delRes == GrdResult.SelectedItems.Count)
                    {
                        await this.ShowMessageAsync("삭제", $"즐겨찾기 {delRes}건 삭제");
                    }
                    else
                    {
                        await this.ShowMessageAsync("삭제", $"즐겨찾기 {GrdResult.SelectedItems.Count}건중 {delRes}건 삭제");
                    }
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"즐겨찾기 삭제 오류 {ex.Message}");
            }

            BtnFavSearch_Click(sender, e);
        }

        private void GrdResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var curItem = GrdResult.SelectedItem as Exhibit;

            var siteWindow = new SiteWindow(curItem.Dabom_url);
            siteWindow.Owner = this;
            siteWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            siteWindow.ShowDialog();
        }
    }
}