using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MensajesLibrary
{

    public partial class MsgBoxCtrl : UserControl
    {
        string TituloMessageBox;
        string TextoMessageBox;
        int TiempoMaximo;
        bool MostrarContador;
        public static string TipoMensaje { get; set; }
        public static string Mensaje { get; set; }
        public static string TituloMensaje { get; set; }
        public int tiempo { get; set; }
        public enum MessageBoxResult
        {
            Yes,
            No,
            Ok,
            Cancel,
            Timeout
        }

        public enum MessageType
        {
            Information,
            Warning,
            Error,
            Success, 
            Question, 
            Stop
        }



        private MessageBoxResult _result;
        private int _timeRemaining = 10;
        private Form _containerForm;

        private TaskCompletionSource<MessageBoxResult> _tcs;

        public int timeRemaining;
        private Timer countdownTimer = new Timer();
        private Timer _timer = new Timer();

        public MsgBoxCtrl()
        {
            InitializeComponent();

            countdownTimer = new Timer();
            countdownTimer.Interval = 1000; // 1 segundo
            countdownTimer.Tick += CountdownTimer_Tick;
        }

 
        public MsgBoxCtrl(string Tipo, string Message, string TituloMensaje, int tiempo, bool contador)
        {
            InitializeComponent();

            if (tiempo != 0)
            {
                countdownTimer = new Timer();
                countdownTimer.Interval = 1000; // 1 segundo
                countdownTimer.Tick += CountdownTimer_Tick;
            }
        }



        public MessageBoxResult ShowMessage(string Tipo, string Message, string TituloMensaje, int tiempo = 0, bool contador = false)
        {
            pbInfo.Visible = false;
            pbError.Visible = false;
            pbQue.Visible = false;
            pbWar.Visible = false;
            pbStop.Visible = false;

            TituloMessageBox = TituloMensaje;
            TiempoMaximo = tiempo;
            MostrarContador = contador;
            TextoMessageBox = Message;


            this.lblMessage.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblMessage.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            // Crear formulario contenedor para simular una ventana modal
            _containerForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterScreen,
                Size = new Size(575, 351),
                //BackColor = Color.FromArgb(240, 240, 240),
                ShowInTaskbar = false,
                TopMost = true
            };

            this.Dock = DockStyle.Fill;
            _containerForm.Controls.Add(this);


            if (tiempo > 0)
            {
                TextoMessageBox = string.Concat(Message, $"\r\nEste mensaje se cerrará dentro de [TIEMPO_ESPERA] segundos");
                this.lblMessage.Text = TextoMessageBox.Replace("[TIEMPO_ESPERA]", tiempo.ToString());
                StartCountdown(tiempo);
            }
            else
            {
                this.lblMessage.Text = TextoMessageBox;
            }


            this.lblTitiuloMessage.Text = string.Empty;
            if (!string.IsNullOrEmpty(TituloMensaje))
            {
                this.lblTitiuloMessage.Text = TituloMensaje;
            }

            switch (Tipo)
            {
                case "info":
                    pL1.BackColor = Color.FromArgb(33, 150, 243);//panel la primera línea
                    pbInfo.Visible = true;//mostramos la imagen
                    break;
                case "question":
                    pL1.BackColor = Color.FromArgb(33, 150, 243);//panel la primera línea
                    pbQue.Visible = true;//mostramos la imagen
                    break;
                case "warning":
                    pL1.BackColor = Color.FromArgb(255, 193, 7);//panel la primera línea
                    pbWar.Visible = true;//mostramos la imagen
                    break;
                case "error":
                    pL1.BackColor = Color.FromArgb(244, 67, 54);//panel la primera línea
                    pbError.Visible = true;//most
                    break;
                case "stop":
                    pL1.BackColor = Color.FromArgb(244, 67, 54);//panel la primera línea
                    pbStop.Visible = true;//most
                    break;

                default:
                    lblTitiuloMessage.Text = "";//"Error al seleccionar";
                    break;
            }


            _containerForm.ShowDialog();

            return _result;

        }

        public MessageBoxResult ShowMessage(MessageType messageType, string Message)
        {
            pbInfo.Visible = false;
            pbError.Visible = false;
            pbQue.Visible = false;
            pbWar.Visible = false;
            pbStop.Visible = false;

            TituloMessageBox = "";
            TiempoMaximo = 0;
            MostrarContador = false;
            TextoMessageBox = Message;


            this.lblMessage.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblMessage.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            // Crear formulario contenedor para simular una ventana modal
            _containerForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterScreen,
                Size = new Size(575, 351),
                ShowInTaskbar = false,
                TopMost = true
            };

            this.Dock = DockStyle.Fill;
            _containerForm.Controls.Add(this);


            if (tiempo > 0)
            {
                if (MostrarContador)
                {
                    TextoMessageBox = string.Concat(Message, $"\r\nEste mensaje se cerrará dentro de [TIEMPO_ESPERA] segundos");
                    this.lblMessage.Text = TextoMessageBox.Replace("[TIEMPO_ESPERA]", tiempo.ToString());
                }

                StartCountdown(tiempo);
            }
            else
            {
                this.lblMessage.Text = TextoMessageBox;
            }


            this.lblTitiuloMessage.Text = string.Empty;
            if (!string.IsNullOrEmpty(TituloMensaje))
            {
                this.lblTitiuloMessage.Text = TituloMensaje;
            }

            switch (messageType)
            {
                case MessageType.Information:
                    pL1.BackColor = Color.FromArgb(33, 150, 243);//panel la primera línea
                    pbInfo.Visible = true;//mostramos la imagen
                    break;
                case MessageType.Question:
                    pL1.BackColor = Color.FromArgb(33, 150, 243);//panel la primera línea
                    pbQue.Visible = true;//mostramos la imagen
                    break;
                case MessageType.Warning:
                    pL1.BackColor = Color.FromArgb(255, 193, 7);//panel la primera línea
                    pbWar.Visible = true;//mostramos la imagen
                    break;
                case MessageType.Error:
                    pL1.BackColor = Color.FromArgb(244, 67, 54);//panel la primera línea
                    pbError.Visible = true;//most
                    break;
                case MessageType.Stop:
                    pL1.BackColor = Color.FromArgb(244, 67, 54);//panel la primera línea
                    pbStop.Visible = true;//most
                    break;

                default:
                    lblTitiuloMessage.Text = "";//"Error al seleccionar";
                    break;
            }


            _containerForm.ShowDialog();

            return _result;

        }

        public MessageBoxResult ShowMessage(MessageType messageType, string Message, string TituloMensaje)
        {
            pbInfo.Visible = false;
            pbError.Visible = false;
            pbQue.Visible = false;
            pbWar.Visible = false;
            pbStop.Visible = false;

            TituloMessageBox = TituloMensaje;
            TiempoMaximo = 0;
            MostrarContador = false;
            TextoMessageBox = Message;


            this.lblMessage.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblMessage.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            // Crear formulario contenedor para simular una ventana modal
            _containerForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterScreen,
                Size = new Size(575, 351),
                ShowInTaskbar = false,
                TopMost = true
            };

            this.Dock = DockStyle.Fill;
            _containerForm.Controls.Add(this);


            if (tiempo > 0)
            {
                if (MostrarContador) {
                    TextoMessageBox = string.Concat(Message, $"\r\nEste mensaje se cerrará dentro de [TIEMPO_ESPERA] segundos");
                    this.lblMessage.Text = TextoMessageBox.Replace("[TIEMPO_ESPERA]", tiempo.ToString());
                }

                StartCountdown(tiempo);
            }
            else
            {
                this.lblMessage.Text = TextoMessageBox;
            }


            this.lblTitiuloMessage.Text = string.Empty;
            if (!string.IsNullOrEmpty(TituloMensaje))
            {
                this.lblTitiuloMessage.Text = TituloMensaje;
            }

            switch (messageType)
            {
                case MessageType.Information:
                    pL1.BackColor = Color.FromArgb(33, 150, 243);//panel la primera línea
                    pbInfo.Visible = true;//mostramos la imagen
                    break;
                case MessageType.Question:
                    pL1.BackColor = Color.FromArgb(33, 150, 243);//panel la primera línea
                    pbQue.Visible = true;//mostramos la imagen
                    break;
                case MessageType.Warning:
                    pL1.BackColor = Color.FromArgb(255, 193, 7);//panel la primera línea
                    pbWar.Visible = true;//mostramos la imagen
                    break;
                case MessageType.Error:
                    pL1.BackColor = Color.FromArgb(244, 67, 54);//panel la primera línea
                    pbError.Visible = true;//most
                    break;
                case MessageType.Stop:
                    pL1.BackColor = Color.FromArgb(244, 67, 54);//panel la primera línea
                    pbStop.Visible = true;//most
                    break;

                default:
                    lblTitiuloMessage.Text = "";//"Error al seleccionar";
                    break;
            }


            _containerForm.ShowDialog();

            return _result;

        }

        public MessageBoxResult ShowMessage(MessageType messageType, string Message, string TituloMensaje, int tiempo = 0, bool contador = false)
        {
            pbInfo.Visible = false;
            pbError.Visible = false;
            pbQue.Visible = false;
            pbWar.Visible = false;
            pbStop.Visible = false;

            TituloMessageBox = TituloMensaje;
            TiempoMaximo = tiempo;
            MostrarContador = contador;
            TextoMessageBox = Message;


            this.lblMessage.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblMessage.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            // Crear formulario contenedor para simular una ventana modal
            _containerForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterScreen,
                Size = new Size(575, 351),
                //BackColor = Color.FromArgb(240, 240, 240),
                ShowInTaskbar = false,
                TopMost = true
            };

            this.Dock = DockStyle.Fill;
            _containerForm.Controls.Add(this);


            if (tiempo > 0)
            {
                if (contador) {
                    TextoMessageBox = string.Concat(Message, $"\r\nEste mensaje se cerrará dentro de [TIEMPO_ESPERA] segundos");
                    this.lblMessage.Text = TextoMessageBox.Replace("[TIEMPO_ESPERA]", tiempo.ToString());
                }
                
                StartCountdown(tiempo);
            }
            else
            {
                this.lblMessage.Text = TextoMessageBox;
            }


            this.lblTitiuloMessage.Text = string.Empty;
            if (!string.IsNullOrEmpty(TituloMensaje))
            {
                this.lblTitiuloMessage.Text = TituloMensaje;
            }

            switch (messageType)
            {
                case MessageType.Information:
                    pL1.BackColor = Color.FromArgb(33, 150, 243);//panel la primera línea
                    pbInfo.Visible = true;//mostramos la imagen
                    break;
                case MessageType.Question:
                    pL1.BackColor = Color.FromArgb(33, 150, 243);//panel la primera línea
                    pbQue.Visible = true;//mostramos la imagen
                    break;
                case MessageType.Warning:
                    pL1.BackColor = Color.FromArgb(255, 193, 7);//panel la primera línea
                    pbWar.Visible = true;//mostramos la imagen
                    break;
                case MessageType.Error:
                    pL1.BackColor = Color.FromArgb(244, 67, 54);//panel la primera línea
                    pbError.Visible = true;//most
                    break;
                case MessageType.Stop:
                    pL1.BackColor = Color.FromArgb(244, 67, 54);//panel la primera línea
                    pbStop.Visible = true;//most
                    break;

                default:
                    lblTitiuloMessage.Text = "";//"Error al seleccionar";
                    break;
            }


            _containerForm.ShowDialog();

            return _result;

        }

        public Task<MessageBoxResult> ShowMessageAsync(Control parent, string Tipo, string Message, string TituloMensaje, int tiempo = 0, bool contador = false)
        {
            _tcs = new TaskCompletionSource<MessageBoxResult>();

            pbInfo.Visible = false;
            pbError.Visible = false;
            pbQue.Visible = false;
            pbWar.Visible = false;
            pbStop.Visible = false;

            TituloMessageBox = TituloMensaje;
            TiempoMaximo = tiempo;
            MostrarContador = contador;
            TextoMessageBox = Message;


            this.lblMessage.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblMessage.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            
           

            this.Location = new Point(
               (parent.ClientSize.Width - this.Width) / 2,
               (parent.ClientSize.Height - this.Height) / 2
           );

            parent.Controls.Add(this);
            this.BringToFront();

            if (tiempo > 0)
            {
                if (contador)
                {
                    TextoMessageBox = string.Concat(Message, $"\r\nEste mensaje se cerrará dentro de [TIEMPO_ESPERA] segundos");
                    this.lblMessage.Text = TextoMessageBox.Replace("[TIEMPO_ESPERA]", tiempo.ToString());
                }

                StartCountdown(tiempo);
            }
            else
            {
                this.lblMessage.Text = TextoMessageBox;
            }

            this.lblTitiuloMessage.Text = string.Empty;
            if (!string.IsNullOrEmpty(TituloMensaje))
            {
                this.lblTitiuloMessage.Text = TituloMensaje;
            }

            switch (Tipo)
            {
                case "info":
                    pL1.BackColor = Color.FromArgb(33, 150, 243);//panel la primera línea
                    pbInfo.Visible = true;//mostramos la imagen
                    break;
                case "question":
                    pL1.BackColor = Color.FromArgb(33, 150, 243);//panel la primera línea
                    pbQue.Visible = true;//mostramos la imagen
                    break;
                case "warning":
                    pL1.BackColor = Color.FromArgb(255, 193, 7);//panel la primera línea
                    pbWar.Visible = true;//mostramos la imagen
                    break;
                case "error":
                    pL1.BackColor = Color.FromArgb(244, 67, 54);//panel la primera línea
                    pbError.Visible = true;//most
                    break;
                case "stop":
                    pL1.BackColor = Color.FromArgb(244, 67, 54);//panel la primera línea
                    pbStop.Visible = true;//most
                    break;

                default:
                    lblTitiuloMessage.Text = "";//"Error al seleccionar";
                    break;
            }



            return _tcs.Task;

        }

        private void StartCountdown(int startTimeInSeconds)
        {
            if (countdownTimer == null) { countdownTimer = new Timer(); }
                
            timeRemaining = startTimeInSeconds;
            countdownTimer.Start(); // Iniciar el temporizador
        }

      

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            timeRemaining--; // Decrementar el tiempo restante

            //// Actualizar el texto del TextBox con el tiempo restante
            //TextoMessageBox = timeRemaining.ToString();
            //TextoMessageBox = TextoMessageBox + "\r\nEste mensaje se cerrará dentro de " + timeRemaining.ToString("00") + " segundos";
            string TextoMessageBoxBase = TextoMessageBox;
            TextoMessageBoxBase = TextoMessageBoxBase.Replace("[TIEMPO_ESPERA]", timeRemaining.ToString());
            this.lblMessage.Text = TextoMessageBoxBase;

            //TextoMessageBox = TextoMessageBox;
            // Si el tiempo se agotó, detener el temporizador
            if (timeRemaining == 0)
            {
                TextoMessageBoxBase = TextoMessageBox.Replace("[TIEMPO_ESPERA]", timeRemaining.ToString());
                this.lblMessage.Text = TextoMessageBoxBase;

                countdownTimer.Stop(); // Detener el temporizador
                countdownTimer.Dispose();
                timeRemaining = 0;

                _result = MessageBoxResult.Timeout;

                btnAceptar_Click(sender, e);
              

            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (timeRemaining != 0) { _timer.Stop(); }
            // _tcs.SetResult(MessageBoxResult.No);
            // this.Parent.Controls.Remove(this);

            _result = MessageBoxResult.No;
            _containerForm.Close();

        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (timeRemaining != 0) { _timer.Stop(); }

            // _tcs.SetResult(MessageBoxResult.Yes);
            // this.Parent.Controls.Remove(this);

            _result = MessageBoxResult.Yes;
            _containerForm.Close();

        }


    }


}
