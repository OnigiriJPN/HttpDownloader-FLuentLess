using AltoHttp;
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

namespace HttpDld
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

        HttpDownloader httpDownloader;
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                httpDownloader = new HttpDownloader(URLBoxs.Text, $"{Environment.SpecialFolder.UserProfile + "ダウンロード"}\\{System.IO.Path.GetFileName(URLBoxs.Text)}");
                httpDownloader.DownloadCompleted += HttpDownloader_DownloadCompleted;
                httpDownloader.ProgressChanged += HttpDownloader_ProgressChanged;
                httpDownloader.Start();
            }
            catch (WebException ex) when ((ex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.NotFound)
            {
                MessageBox.Show("指定したURLは、リソースが見つかりません。\nリソースが消された・別サイトへ移転した可能性があります。");
            }
            catch (WebException ex) when ((ex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.RequestTimeout)
            {
                MessageBox.Show("指定したURLでサーバーの応答がありません。時間をおいてからもう一度お試しください。");
            }
            catch (WebException ex) when ((ex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.BadRequest)
            {
                MessageBox.Show("指定したURLで不正なリクエストが検出されました。");
            }
            catch (WebException ex) when ((ex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.Unauthorized)
            {
                MessageBox.Show("指定したURLでサーバー側で認証していないようです。認証をしてからもう一度お試しください");
            }
            catch (WebException ex) when ((ex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.Forbidden)
            {
                MessageBox.Show("指定したURLでアクセスが禁止されております。");
            }
            catch (WebException ex) when ((ex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.TooManyRequests)
            {
                MessageBox.Show("指定したURLでアクセス回数が多すぎます。");
            }
            catch (WebException ex) when ((ex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.InternalServerError)
            {
                MessageBox.Show("指定したURLでサーバー側の内部エラーが発生しました。");
            }
            catch (WebException ex) when ((ex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.BadGateway)
            {
                MessageBox.Show("指定したURLでサーバー側の正常な応答を受け取ることができませんでした。");
            }
            catch(WebException ex)
            {
                MessageBox.Show(this, $"開始操作にエラーが発生しました。\n理由:{ex.Message}\nエラータイプ: {ex.GetType().FullName}", "エラーが発生しました。");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,$"開始操作にエラーが発生しました。\n理由:{ex.Message}\nエラータイプ: {ex.GetType().FullName}", "エラーが発生しました。");
            }
        }

        private void HttpDownloader_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            try
            {
                Progress.Value = (int)e.Progress;
                lblPercent.Text = $"{e.Progress.ToString("0.00")} %";
                lblSpeed.Text = string.Format("{0} MB/s", (e.SpeedInBytes / 1024d / 1024d).ToString("0.00"));
                lblSize.Text = string.Format("{0} MB/s", (httpDownloader.TotalBytesReceived / 1024d / 1024d).ToString("0.00"));
                lblStatus.Text = "ダウンロード中...しばらくお待ちください。";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"ダウンロード中操作にエラーが発生しました。\n理由:{ex.Message}\nエラータイプ: {ex.GetType().FullName}", "エラーが発生しました。");
            }
        }

        private void HttpDownloader_DownloadCompleted(object? sender, EventArgs e)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                lblStatus.Text = "完了！";
                lblPercent.Text = "100%";
            });
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            if(httpDownloader != null)
            httpDownloader.Pause();
        }

        private void btnResume_Click(object sender, RoutedEventArgs e)
        {
            if (httpDownloader != null)
                httpDownloader.Resume();
        }
    }
}