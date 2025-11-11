using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Windows;

namespace otoface
{
    public class CsvGenerator()
    {
        public void GenerateCsvFromEventsAndJson(string jsonFilePath, List<KeyEvent> keyEvents)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                DefaultExt = "csv",
                FileName = "KeyEvents"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                var groups = JsonConvert.DeserializeObject<List<Group>>(File.ReadAllText(jsonFilePath));
                var csvLines = new List<string>();

                csvLines.Add("Vocaloid Motion Data 0002");
                csvLines.Add("YYB Hatsune Miku_def");
                csvLines.Add("Motion,bone,x,y,z,rx,ry,rz,x_p1x,x_p1y,x_p2x,x_p2y,y_p1x,y_p1y,y_p2x,y_p2y,z_p1x,z_p1y,z_p2x,z_p2y,r_p1x,r_p1y,r_p2x,r_p2y");
                csvLines.Add("Expression,name,fact");

                foreach (var ke in keyEvents)
                {
                    var group = groups.FirstOrDefault(g => g.GroupName == ke.Key);
                    if (group != null)
                    {
                        foreach (var bone in group.Bones)
                        {
                            var line1 = $"{ke.Frame},{bone.BoneName},{(ke.EventType == "Down" ? "0" : bone.Value.ToString())}";
                            csvLines.Add(line1);
                            var line2 = $"{ke.Frame + int.Parse(group.FadeFrame)},{bone.BoneName},{(ke.EventType == "Down" ? bone.Value.ToString() : "0")}";
                            csvLines.Add(line2);
                        }
                    }
                }

                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                File.WriteAllLines(saveFileDialog.FileName, csvLines, Encoding.GetEncoding("shift_jis"));
                MessageBox.Show($"CSVが保存されました: {saveFileDialog.FileName}", "保存完了", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}