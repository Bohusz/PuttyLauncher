using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using PuttyLauncher.Properties;

namespace PuttyLauncher {
    public partial class PuttyLauncherForm : Form {
        private const string PUTTY_EXE = @"C:\Program Files\PuTTY\putty.exe";
        private const int ButtonWidth = 200;
        private const int ButtonHeight = 80;
        private const int MaxButtonsInColumn = 10;

        private readonly PuttyConfigurationsSource _configurationsSource = new PuttyConfigurationsSource();

        public PuttyLauncherForm() {
            InitializeComponent();
            SuspendLayout();
                InitializeButtons();
            ResumeLayout();
        }
        
        private void InitializeButtons() {
            var configurationCount = _configurationsSource.ConfigurationCount();
            var buttonRowsCount    = configurationCount > MaxButtonsInColumn ? MaxButtonsInColumn : configurationCount;
            var buttonColumnsCount = (configurationCount / MaxButtonsInColumn) + (configurationCount % MaxButtonsInColumn > 0 ? 1 : 0);
            
            for (var i = 0; i < configurationCount; i++) {
                var posX   = i / MaxButtonsInColumn;
                var posY   = i % MaxButtonsInColumn;
                Controls.Add(
                    CreateButton(posX, posY, _configurationsSource.Configurations()[i])
                );
            }

            ClientSize = new Size(buttonColumnsCount * ButtonWidth, buttonRowsCount * ButtonHeight);
        }

        private Button CreateButton(int posX, int posY, PuttyConfigurationItem definition) {
            var       buttonImage = Resources.anchor;
            var       buttonText  = BreakLongText(definition.label);
            var       buttonName  = string.Format("buttonX{0}Y{1}", posX, posY);
            //Language Level 6: var       buttonName  = $"buttonX{posX}Y{posY}";

            var button = new Button {
                Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 238)
              , Image = buttonImage
              , ImageAlign = ContentAlignment.MiddleLeft
              , Location = new Point(posX * ButtonWidth, posY * ButtonHeight)
              , Margin = new Padding(0)
              , Name = "button1"
              , Size = new Size(ButtonWidth, ButtonHeight)
              , TabIndex = 0
              , Text = buttonText
              , TextAlign = ContentAlignment.MiddleRight
              , UseVisualStyleBackColor = true
            };
            button.Click += (sender, args) => LaunchPutty(definition.label); 
            return button;
        }

        private void LaunchPutty(string configurationName) {
            Process.Start(PUTTY_EXE, "-load \"" + configurationName + "\"");
            Close();
        }

        private static string BreakLongText(string text) {
            const int minEnd = 13;
            const int maxEnd = 16;

            if (text.Length < minEnd) return text;
            var brokenText = BreakText(text, minEnd, maxEnd);
            return brokenText.Item2 == ""
                    ? brokenText.Item1
                    : string.Concat(brokenText.Item1,"\r\n", brokenText.Item2)
                ;
        }
        private static Tuple<string, string> BreakText(string text, int from, int to) {
            for (var end = from; end < to && end < text.Length; end++) {
                var spacePosition = text.Substring(0, end).LastIndexOf(' ');
                if (spacePosition > 0) {
                    return Tuple.Create(
                        text.Substring(0, spacePosition)
                      , text.Substring(spacePosition + 1)
                    );
                }
            }
            if (text.Length < to) return Tuple.Create(text, "");
            
            var followingSpacePosition = text.Substring(to).IndexOf(' ');
            return Tuple.Create(
                string.Concat(text.Substring(0, to), "\u2026" /* ellipsis */)
              , followingSpacePosition < 0 ? "" : text.Substring(followingSpacePosition + 1)
            );
        }

        private void PuttyLauncherForm_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) {
                Close();
            }
        }
    }
}