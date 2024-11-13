using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;

namespace TypeWriter
{
    internal class AppConfigSource
    {
        private readonly string _path;
        private AppConfig _appConfig = null!;
        private readonly JsonSerializerOptions _options;
        private readonly object _syncObj;

        public AppConfigSource()
        {
            _path = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath)!, "AppConfig.json");
            _syncObj = new object();
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
            if (_appConfig == null)
            {
                lock (_syncObj)
                {
                    if (_appConfig == null)
                    {
                        try
                        {
                            if (File.Exists(_path))
                            {
                                var json = File.ReadAllText(_path, Encoding.UTF8);
                                _appConfig = JsonSerializer.Deserialize<AppConfig>(json, _options)!;
                            }
                            else
                            {
                                GiveDefaultValue();
                            }
                        }
                        catch
                        {
                            GiveDefaultValue();
                        }
                    }
                }
            }

            return _appConfig!;

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
                _appConfig.TypeBoxHeight = 30;
            }
        }

        public void SaveConfig(AppConfig config)
        {
            ArgumentNullException.ThrowIfNull(nameof(config));
            var configJson = JsonSerializer.Serialize(config, _options);
            _appConfig = JsonSerializer.Deserialize<AppConfig>(configJson, _options)!;
            File.WriteAllText(_path, configJson, Encoding.UTF8);
        }
    }
}