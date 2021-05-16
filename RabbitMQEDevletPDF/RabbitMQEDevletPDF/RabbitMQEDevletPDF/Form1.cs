using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Internal;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DocumentCommon;
using RabbitMQ.Client;

namespace RabbitMQEDevletPDF
{
    public partial class documentCreaterForm : Form
    {
        private RabbitMQClientService rabbitMqClientService = new RabbitMQClientService();
        private readonly string createDocumentQueue = "create_document_queue";
        public string createdDocumentQueue = "created_document_queue";


        public documentCreaterForm()
        {
            InitializeComponent();
        }

        private void connectBtn_Click(object sender, EventArgs e)
        {
            rabbitMqClientService.CreateConnection(connectionStringTxt.Text);
            AddLog("Connected.");
            rabbitMqClientService.MakeDeclaration(createDocumentQueue);
            createPdfBtn.Enabled = true; // bağlantı sağlandığında pdf oluşturma butonu aktif edilsin.
        }

        
        private void AddLog(string logInfo)
        {
            if (logInfotxt.InvokeRequired)
            {
                logInfotxt.Invoke(new Action(() => AddLog(logInfo))); // sürekli kendi metodunu tetikleyerek çağırıldıkça yazdırmayı sağlıyor.
                return;
            }

            logInfo = $"[{DateTime.Now:dd.MM.yyyy HH:mm:ss}] - {logInfo}";
            logInfotxt.AppendText($"{logInfo}\n");

            logInfotxt.SelectionStart = logInfotxt.Text.Length;
            logInfotxt.ScrollToCaret();
        }

        private void createPdfBtn_Click(object sender, EventArgs e)
        {
            var model = new CreateDocumentModel()
            {
                UserId = 1,
                DocumentType = DocumentTypes.Pdf
            };

            rabbitMqClientService.WriteToQueue(createDocumentQueue, model);
            AddLog("Published to queue.");

            Task.Delay(5000);
           rabbitMqClientService.ConsumeFromQueue(createdDocumentQueue);

        }
    }
}
