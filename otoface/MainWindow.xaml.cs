using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Windows.Media;

namespace otoface
{
    public partial class MainWindow : Window
    {   
        private Stopwatch timer;
        private Key prevKey;
        private List<KeyEvent> keyInputs = new List<KeyEvent>();
        // private MediaPlayer mediaPlayer = new MediaPlayer();
        private string selectedFilePath;

        public MainWindow()
        {
            InitializeComponent();

            var manager = new JsonDataManager();
            var groups = manager.LoadGroups("faceGroup.json");
            groupKey.ItemsSource = groups;

            timer = new Stopwatch();
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Play();
            timer.Restart();
            keyInputsList.Items.Clear();
            keyInputs.Clear();
            this.KeyDown += MainWindow_KeyDown; // キー入力イベントを登録
            this.KeyUp += MainWindow_KeyUp;
        }

        private void processEvent(Key k, string eventType)
        {
            var timeElapsedMilliseconds = timer.ElapsedMilliseconds;
            var frameElapsed = (int)(((float)timeElapsedMilliseconds / 1000) * 30);
            var group = "";

            char keyChar = (char)('A' + (k - Key.A));
            string keyValue = keyChar.ToString();

            // DataGrid の ItemsSource を検索して、キーに対応する値を見つける
            var itemsSource = groupKey.ItemsSource as IEnumerable<Group>;
            if (itemsSource != null)
            {
                foreach (var item in itemsSource)
                {
                    if (item.Key == keyValue)
                    {
                        group = item.GroupName;
                        break;
                    }

                }
            }

            if (group == "")
            {
                return;
            }

            var ke = new KeyEvent(frameElapsed, group, eventType);
            keyInputs.Add(ke);
            var inputText = $"{group}, {frameElapsed}, {eventType}";
            keyInputsList.Items.Add(inputText);
            keyInputsList.ScrollIntoView(inputText);

        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            var k = e.Key;
            if (timer.IsRunning && k >= Key.A && k <= Key.Z && prevKey != k)
            {
                processEvent(k, "ON");
            }
            prevKey = k;
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            var k = e.Key;
            if (timer.IsRunning)
            {
                processEvent(k, "OFF");
            }
            prevKey = Key.None;
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            mediaPlayer.Stop();
            this.KeyDown -= MainWindow_KeyDown; // キー入力イベントを解除
            this.KeyUp -= MainWindow_KeyUp;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

            MessageBoxResult result = MessageBox.Show("データを保存しますか？", "OtoFace", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                savejson();
            }
        }

        private void savejson()
        {
            // DataGrid の ItemsSource を適切な型にキャスト
            var groups = groupKey.ItemsSource as ObservableCollection<Group>;

            if (groups != null)
            {
                // シリアライズ設定を準備（空文字列プロパティを無視するカスタム ContractResolver を使用）
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new IgnoreEmptyStringPropertiesResolver(),
                    Formatting = Formatting.Indented // 読みやすいフォーマットで出力
                };

                // オブジェクトを JSON 文字列にシリアライズ
                string json = JsonConvert.SerializeObject(groups, settings);

                // JSON 文字列をファイルに保存
                // 保存先のファイルパスを適宜指定してください
                string filePath = "faceGroup.json";
                File.WriteAllText(filePath, json);

                // 保存完了の通知（必要に応じて）
                MessageBox.Show("保存しました。", "OtoFace", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // データソースが null の場合の処理（エラーメッセージ表示など）
                MessageBox.Show("保存するデータがありません。", "OtoFace", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void groupKey_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (groupKey.SelectedItem is Group selectedGroup)
            {

                if (selectedGroup.Bones == null || selectedGroup.Bones.Count == 0)
                {
                    selectedGroup.Bones = new ObservableCollection<Bone>();
                }
                boneGroup.ItemsSource = selectedGroup.Bones;
            }
            else
            {
                boneGroup.ItemsSource = null;
            }
        }

        private void AddGroupButton_Click(object sender, RoutedEventArgs e)
        {
            var inputDialog = new GroupInputDialog();
            if (inputDialog.ShowDialog() == true)
            {
                var newItem = new Group
                {
                    GroupName = inputDialog.Expression,
                    Key = inputDialog.Key,
                    FadeFrame = inputDialog.FadeFrame                };
                // ここで newItem を DataGrid の ItemsSource に追加
                (groupKey.ItemsSource as ObservableCollection<Group>)?.Add(newItem);
            }
        }

        private void AddBoneButton_Click(object sender, RoutedEventArgs e)
        {
            var inputDialog = new BoneInputDialog();
            if (inputDialog.ShowDialog() == true)
            {
                var newItem = new Bone
                {
                    BoneName = inputDialog.BoneName,
                    Value = inputDialog.Value,
                    Parts = inputDialog.Parts
                };
                // ここで newItem を DataGrid の ItemsSource に追加
                (boneGroup.ItemsSource as ObservableCollection<Bone>)?.Add(newItem);

            }
        }

        private void DeleteGroupButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedGroup = groupKey.SelectedItem as Group;
            if (selectedGroup != null)
            {
                // 確認ダイアログを表示
                MessageBoxResult result = MessageBox.Show("選択された行を削除しますか？", "確認", MessageBoxButton.YesNo, MessageBoxImage.Question);

                // ユーザーが「はい」を選択した場合のみ削除を実行
                if (result == MessageBoxResult.Yes)
                {
                    var groups = groupKey.ItemsSource as ObservableCollection<Group>;
                    if (groups != null)
                    {
                        groups.Remove(selectedGroup);
                    }
                }
            }
            else
            {
                // 何も選択されていない場合のメッセージ
                MessageBox.Show("削除する行が選択されていません。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteBoneButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedGroup = boneGroup.SelectedItem as Bone;
            if (selectedGroup != null)
            {
                // 確認ダイアログを表示
                MessageBoxResult result = MessageBox.Show("選択された行を削除しますか？", "確認", MessageBoxButton.YesNo, MessageBoxImage.Question);

                // ユーザーが「はい」を選択した場合のみ削除を実行
                if (result == MessageBoxResult.Yes)
                {
                    var groups = boneGroup.ItemsSource as ObservableCollection<Bone>;
                    if (groups != null)
                    {
                        groups.Remove(selectedGroup);
                    }
                }
            }
            else
            {
                // 何も選択されていない場合のメッセージ
                MessageBox.Show("削除する行が選択されていません。", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SelectMovieFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Video Files (*.mp4;*.avi)|*.mp4;*.avi"; // 音声ファイルのフィルタ
            if (openFileDialog.ShowDialog() == true)
            {

                selectedFilePath = openFileDialog.FileName; // 選択されたファイルのパスを保存
                mediaPlayer.Source = new Uri(openFileDialog.FileName);
                pathLabel.Content = selectedFilePath;
            }
        }

        private void outputButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("csv出力する前に表情データをjsonに保存する必要があります。\n保存してよろしいですか？", "OtoFace", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                savejson();
                CsvGenerator CsvGen = new CsvGenerator();
                CsvGen.GenerateCsvFromEventsAndJson("faceGroup.json", keyInputs);
            }
        }

        private void keyInputsList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = keyInputsList.SelectedIndex;
            KeyEvent targetKeyInput = keyInputs[index];
            string faceGroup = targetKeyInput.Key;
            int frame = targetKeyInput.Frame;
            string eventType = targetKeyInput.EventType;

            KeyEventEditDialog keyEventEditDialog = new KeyEventEditDialog(faceGroup, frame, eventType);
            if (keyEventEditDialog.ShowDialog() == true)
            {
                KeyEvent newKeyEvent = new KeyEvent(int.Parse(keyEventEditDialog.Frame), keyEventEditDialog.FaceGroup, keyEventEditDialog.EventType); // フレームがintであることはkeyEventEditDialogクラス内で保証済み
                keyInputs[index] = newKeyEvent;
                var newText = $"{keyEventEditDialog.FaceGroup}, {keyEventEditDialog.Frame}, {keyEventEditDialog.EventType}";
                keyInputsList.Items[index] = newText;
            }
        }
    }
}
