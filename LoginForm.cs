using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using Reborn;

namespace Client
{
    public class LoginForm : Form
    {
        private IntPtr _mainHandle;
        private Guna2BorderlessForm _borderlessForm = null!;
        private Guna2ShadowForm _shadowForm = null!;
        private Guna2Panel _cardPanel = null!;
        private Guna2TextBox _keyTextBox = null!;
        private Guna2Button _loginButton = null!;
        private Guna2Button _pasteButton = null!;
        private Label _titleLabel = null!;
        private Label _subtitleLabel = null!;
        private Label _statusLabel = null!;
        private System.ComponentModel.IContainer components = new System.ComponentModel.Container();
        private Guna2ControlBox _closeBox = null!;
        private Guna2ControlBox _minimizeBox = null!;

        //==========KEYAUTH SETUP====================================================
        public static api? KeyAuthApp;
        private const string AppName = "Aidaouisedik28's Application";
        private const string OwnerId = "5p4oCVTHHX";
        private const string Secret = "64951a8e38872adf4acc7bddfd1adefd1beed755095aca91932772b180509e3e";
        private const string Version = "1.0";
        //===========================================================================

        public LoginForm(IntPtr mainHandle)
        {
            _mainHandle = mainHandle;
            InitializeUi();
            InitializeKeyAuth();
        }

        private void InitializeUi()
        {
            this.AutoScaleMode = AutoScaleMode.None;
            this.ClientSize = new Size(480, 360);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(10, 13, 18);
            this.TopMost = true;

            // Borderless & Shadow
            _borderlessForm = new Guna2BorderlessForm(this.components)
            {
                ContainerControl = this,
                BorderRadius = 14,
                HasFormShadow = true,
                TransparentWhileDrag = true
            };

            _shadowForm = new Guna2ShadowForm(this.components)
            {
                TargetForm = this,
                ShadowColor = Color.FromArgb(255, 23, 68)
            };

            // Main Card Panel
            _cardPanel = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(14, 18, 25),
                BorderColor = Color.FromArgb(255, 23, 68),
                BorderRadius = 14,
                BorderThickness = 1,
                Padding = new Padding(24)
            };
            this.Controls.Add(_cardPanel);

            // Close & Minimize
            _closeBox = new Guna2ControlBox
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(436, 12),
                Size = new Size(30, 30),
                FillColor = Color.Transparent,
                IconColor = Color.FromArgb(255, 82, 82),
                HoverState = { FillColor = Color.FromArgb(255, 23, 68), IconColor = Color.White }
            };
            _closeBox.Click += (s, e) => { Environment.Exit(0); };

            _minimizeBox = new Guna2ControlBox
            {
                ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MinimizeBox,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(400, 12),
                Size = new Size(30, 30),
                FillColor = Color.Transparent,
                IconColor = Color.FromArgb(139, 148, 158),
                HoverState = { FillColor = Color.FromArgb(33, 38, 45), IconColor = Color.White }
            };

            _cardPanel.Controls.Add(_closeBox);
            _cardPanel.Controls.Add(_minimizeBox);

            // Title Label
            _titleLabel = new Label
            {
                Text = "KRISHU X CHEATS",
                Font = new Font("Segoe UI Black", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 23, 68),
                Location = new Point(24, 35),
                AutoSize = true
            };
            _cardPanel.Controls.Add(_titleLabel);

            // Subtitle Label
            _subtitleLabel = new Label
            {
                Text = "OB55 UPDATED VIP INTERNAL • FREE FIRE",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(110, 118, 129),
                Location = new Point(27, 72),
                AutoSize = true
            };
            _cardPanel.Controls.Add(_subtitleLabel);

            // Key Input TextBox
            _keyTextBox = new Guna2TextBox
            {
                Location = new Point(30, 130),
                Size = new Size(320, 42),
                BorderRadius = 8,
                BorderColor = Color.FromArgb(48, 54, 61),
                BorderThickness = 1,
                FillColor = Color.FromArgb(22, 27, 34),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                PlaceholderText = "ENTER LICENSE KEY...",
                PlaceholderForeColor = Color.FromArgb(72, 79, 88)
            };
            _keyTextBox.FocusedState.BorderColor = Color.FromArgb(255, 23, 68);
            _cardPanel.Controls.Add(_keyTextBox);

            // Paste Button
            _pasteButton = new Guna2Button
            {
                Text = "PASTE",
                Location = new Point(360, 130),
                Size = new Size(80, 42),
                BorderRadius = 8,
                BorderThickness = 1,
                BorderColor = Color.FromArgb(48, 54, 61),
                FillColor = Color.FromArgb(33, 38, 45),
                ForeColor = Color.FromArgb(139, 148, 158),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _pasteButton.Click += (s, e) =>
            {
                if (Clipboard.ContainsText())
                {
                    _keyTextBox.Text = Clipboard.GetText().Trim();
                }
            };
            _cardPanel.Controls.Add(_pasteButton);

            // Status Label
            _statusLabel = new Label
            {
                Text = "Enter VIP key to continue",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(139, 148, 158),
                Location = new Point(30, 185),
                Size = new Size(410, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };
            _cardPanel.Controls.Add(_statusLabel);

            // Login Button
            _loginButton = new Guna2Button
            {
                Text = "LOGIN TO PANEL",
                Location = new Point(30, 225),
                Size = new Size(410, 48),
                BorderRadius = 8,
                BorderColor = Color.FromArgb(255, 82, 82),
                BorderThickness = 1,
                FillColor = Color.FromArgb(255, 23, 68),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _loginButton.HoverState.FillColor = Color.FromArgb(213, 0, 0);
            _loginButton.Click += LoginButton_Click;
            _cardPanel.Controls.Add(_loginButton);

            // Footer
            var footer = new Label
            {
                Text = "Protected by Krishu Cheats VIP",
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(72, 79, 88),
                Location = new Point(30, 310),
                Size = new Size(410, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            _cardPanel.Controls.Add(footer);
        }

        private void InitializeKeyAuth()
        {
            Task.Run(() =>
            {
                try
                {
                    if (OwnerId.Length == 10 && Secret.Length == 64 && AppName != "Application Name here")
                    {
                        KeyAuthApp = new api(AppName, OwnerId, Secret, Version);
                        KeyAuthApp.init();

                        if (this.IsHandleCreated)
                        {
                            this.Invoke((MethodInvoker)(() =>
                            {
                                _statusLabel.Text = "Connected to KeyAuth Server";
                                _statusLabel.ForeColor = Color.FromArgb(0, 230, 118);
                            }));
                        }
                    }
                    else
                    {
                        if (this.IsHandleCreated)
                        {
                            this.Invoke((MethodInvoker)(() =>
                            {
                                _statusLabel.Text = "Dev Mode Active (KeyAuth not configured)";
                                _statusLabel.ForeColor = Color.FromArgb(255, 179, 0);
                            }));
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (this.IsHandleCreated)
                    {
                        this.Invoke((MethodInvoker)(() =>
                        {
                            _statusLabel.Text = "Note: " + ex.Message;
                        }));
                    }
                }
            });
        }

        private async void LoginButton_Click(object? sender, EventArgs e)
        {
            string key = _keyTextBox.Text.Trim();
            if (string.IsNullOrEmpty(key))
            {
                _statusLabel.Text = "Please enter your license key!";
                _statusLabel.ForeColor = Color.FromArgb(255, 82, 82);
                return;
            }

            _statusLabel.Text = "Authenticating...";
            _statusLabel.ForeColor = Color.FromArgb(0, 229, 255);
            _loginButton.Enabled = false;

            bool authSuccess = false;
            string authMsg = "";

            await Task.Run(() =>
            {
                try
                {
                    if (KeyAuthApp != null)
                    {
                        KeyAuthApp.license(key);
                        if (KeyAuthApp.response.success)
                        {
                            authSuccess = true;
                            authMsg = "Login Successful!";
                        }
                        else
                        {
                            authSuccess = false;
                            authMsg = KeyAuthApp.response.message ?? "Invalid License Key.";
                        }
                    }
                    else
                    {
                        // Dev mode fallback
                        authSuccess = true;
                        authMsg = "Login Successful (Dev Mode)";
                    }
                }
                catch (Exception ex)
                {
                    authSuccess = false;
                    authMsg = "Auth Error: " + ex.Message;
                }
            });

            if (authSuccess)
            {
                _statusLabel.Text = authMsg;
                _statusLabel.ForeColor = Color.FromArgb(0, 230, 118);

                await Task.Delay(500);

                this.Hide();
                var mainPanel = new MainPanelForm(_mainHandle);
                mainPanel.ShowDialog();
                this.Close();
            }
            else
            {
                _statusLabel.Text = authMsg;
                _statusLabel.ForeColor = Color.FromArgb(255, 82, 82);
                _loginButton.Enabled = true;
            }
        }
    }
}
