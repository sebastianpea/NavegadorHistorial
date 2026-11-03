using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NavegadorHistorial
{
    public partial class FormPrincipal : Form
    {
    private readonly Navegador navegador = new();

        private readonly Button btnAtras = new() { Text = "◀ Atrás", AutoSize = true };
        private readonly Button btnAdelante = new() { Text = "Adelante ▶", AutoSize = true };
        private readonly TextBox txtDireccion = new() { Width = 420 };
        private readonly Button btnIr = new() { Text = "Ir", AutoSize = true };
        private readonly Label lblTitulo = new() { AutoSize = true, Font = new Font("Segoe UI", 16F, FontStyle.Bold), Margin = new Padding(10, 14, 3, 6) };
        private readonly Label lblContenido = new() { AutoSize = true, MaximumSize = new Size(560, 0), Margin = new Padding(10, 3, 3, 12) };
        private readonly FlowLayoutPanel panelEnlaces = new() { FlowDirection = FlowDirection.TopDown, AutoSize = true, Margin = new Padding(10, 3, 3, 3) };
        private readonly ListBox lstAtras = new() { Dock = DockStyle.Fill, IntegralHeight = false, Font = new Font("Consolas", 9.5F) };
        private readonly ListBox lstAdelante = new() { Dock = DockStyle.Fill, IntegralHeight = false, Font = new Font("Consolas", 9.5F) };
        private readonly TextBox txtBitacora = new() { Dock = DockStyle.Fill, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical, Font = new Font("Consolas", 9.5F), BackColor = Color.White };

        public FormPrincipal()
        {
            Text = "Tarea 3.3 - Historial de navegación con pilas";
            Size = new Size(1150, 680);
            MinimumSize = new Size(900, 520);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 10F);

            var barra = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
            barra.Controls.AddRange(new Control[] { btnAtras, btnAdelante, txtDireccion, btnIr });

            var pagina = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            pagina.Controls.AddRange(new Control[] { lblTitulo, lblContenido, new Label { Text = "Enlaces:", AutoSize = true, Margin = new Padding(10, 3, 3, 3) }, panelEnlaces });

            var pilas = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 6 };
            pilas.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pilas.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            pilas.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pilas.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            pilas.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pilas.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            pilas.Controls.Add(Titulo("Pila ATRÁS (cima arriba)"), 0, 0);
            pilas.Controls.Add(lstAtras, 0, 1);
            pilas.Controls.Add(Titulo("Pila ADELANTE (cima arriba)"), 0, 2);
            pilas.Controls.Add(lstAdelante, 0, 3);
            pilas.Controls.Add(Titulo("Operaciones"), 0, 4);
            pilas.Controls.Add(txtBitacora, 0, 5);

            var cuerpo = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
            cuerpo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            cuerpo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            cuerpo.Controls.Add(pagina, 0, 0);
            cuerpo.Controls.Add(pilas, 1, 0);

            var principal = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, Padding = new Padding(6) };
            principal.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            principal.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            principal.Controls.Add(barra, 0, 0);
            principal.Controls.Add(cuerpo, 0, 1);
            Controls.Add(principal);

            btnAtras.Click += (s, e) => Ejecutar("Atrás", navegador.Retroceder);
            btnAdelante.Click += (s, e) => Ejecutar("Adelante", navegador.Avanzar);
            btnIr.Click += (s, e) => Ir(txtDireccion.Text);
            txtDireccion.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { Ir(txtDireccion.Text); e.SuppressKeyPress = true; } };

            Ir(SitioSimulado.PaginaInicial);
        }

        private void Ir(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return;
            Ejecutar($"Visitar {url.Trim()}", () => navegador.Visitar(url.Trim().ToLowerInvariant()));
        }

        /// <summary>Ejecuta una operación del navegador, la registra y actualiza toda la ventana.</summary>
        private void Ejecutar(string descripcion, Action operacion)
        {
            try
            {
                operacion();
                txtBitacora.AppendText($"{descripcion} -> actual: {navegador.Actual}{Environment.NewLine}");
            }
            catch (InvalidOperationException ex)
            {
                txtBitacora.AppendText($"{descripcion}: {ex.Message}{Environment.NewLine}");
            }
            Actualizar();
        }

        private void Actualizar()
        {
            Pagina p = SitioSimulado.Obtener(navegador.Actual ?? SitioSimulado.PaginaInicial);
            txtDireccion.Text = navegador.Actual;
            lblTitulo.Text = p.Titulo;
            lblContenido.Text = p.Contenido;

            panelEnlaces.Controls.Clear();
            foreach (string enlace in p.Enlaces)
            {
                var link = new LinkLabel { Text = enlace, AutoSize = true, Margin = new Padding(3, 4, 3, 4) };
                link.LinkClicked += (s, e) => Ir(enlace);
                panelEnlaces.Controls.Add(link);
            }

            lstAtras.DataSource = navegador.PilaAtras.ToList();
            lstAdelante.DataSource = navegador.PilaAdelante.ToList();
            btnAtras.Enabled = navegador.PuedeRetroceder;
            btnAdelante.Enabled = navegador.PuedeAvanzar;
        }

        private static Label Titulo(string t) => new() { Text = t, AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold), Margin = new Padding(3, 8, 3, 3) };
    }
}
