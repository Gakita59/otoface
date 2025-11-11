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

namespace otoface
{
    /// <summary>
    /// KeyEventEditDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class KeyEventEditDialog : Window
    {
        public string FaceGroup => txtFaceGroup.Text;
        public string Frame => txtFrame.Text;
        public string EventType => radioOn.IsChecked == true ? "ON" : "OFF";
        public KeyEventEditDialog(string faceGroup, int frame, string eventType)
        {
            InitializeComponent();
            txtFaceGroup.Text = faceGroup;
            txtFrame.Text = frame.ToString();
            if (eventType == "ON")
            {
                radioOn.IsChecked = true;
            } else
            {
                radioOff.IsChecked = true;
            }

        }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(Frame, out int num))
            {
                // 数値がintになっている → ダイアログを閉じる
                this.DialogResult = true;
            } else
            {   // 数値がintになっていない → メッセージを表示して再入力を促す
                MessageBox.Show(
                    "フレームには数値を入力してください。",
                    "入力エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
            }
        }
    }
}
