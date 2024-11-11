using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;
using Xueban.TypeWriter;

namespace TypeWriter
{
    internal class AppConfigSource
    {
        private string _path;
        private AppConfig _appConfig = null!;
        private JsonSerializerOptions _options;

        public AppConfigSource()
        {
            _path = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath)!, "AppConfig.json");
            _options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters =
                {
                    new ColorJsonConverter(),
                    new FontInfoJsonConverter()
                }
            };
        }

        public AppConfig GetConfig()
        {
            void GiveDefaultValue()
            {
                _appConfig = new AppConfig();
                _appConfig.BackColor = Colors.White;
                _appConfig.ToTypeFont = new WpfColorFontDialog.FontInfo();
                _appConfig.ToTypeFont.BrushColor = new SolidColorBrush(Colors.Black);
                _appConfig.ToTypeFont.Stretch = FontStretches.Normal;
                _appConfig.ToTypeFont.Weight = FontWeights.Regular;
                _appConfig.ToTypeFont.Style = FontStyles.Normal;
                _appConfig.ToTypeFont.Family = new FontFamily("Consolas");
                _appConfig.TypedFont = new WpfColorFontDialog.FontInfo();
                _appConfig.TypedFont.BrushColor = new SolidColorBrush(Colors.Black);
                _appConfig.TypedFont.Stretch = FontStretches.Normal;
                _appConfig.TypedFont.Weight = FontWeights.Regular;
                _appConfig.TypedFont.Style = FontStyles.Normal;
                _appConfig.TypedFont.Family = new FontFamily("Consolas");
                _appConfig.TypeBoxWidth = 500;
                _appConfig.TypeBoxHeight = 50;
            }

            if (_appConfig == null)
            {
                try
                {
                    if (File.Exists(_path))
                    {
                        var json = File.ReadAllText(_path, Encoding.UTF8);
                        JsonConvert.DeserializeObject<AppConfig>(json,new JsonSerializerSettings()
                        {
                            Converters =
                            {
                                new ColorJsonConverter2(),
                                new FontInfoJsonConverter2(),
                            }
                        });
                        //_appConfig = JsonSerializer.Deserialize<AppConfig>(json, _options)!;
                    }
                    else
                    {
                        GiveDefaultValue();
                    }
                }
                catch(Exception ex)
                {
                    GiveDefaultValue();
                }
            }
            return _appConfig!;
        }

        public void SaveConfig(AppConfig config)
        {
            ArgumentNullException.ThrowIfNull(nameof(config));
            _appConfig = config;
            var configJson = System.Text.Json.JsonSerializer.Serialize(_appConfig, _options);
            File.WriteAllText(_path, configJson, Encoding.UTF8);
        }
    }
}