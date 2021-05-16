using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class CreateExcelMessage // bu class ı publisher yaptığımız taraftan mesaj olarak göndermek çok malyetli ve sağlıksız olacağı için, subscriber tarafından
    {// veritabanından çekilebilmesi için, veri çekerken kullnacağı bilgileri mesaj yoluyla gönderiyoruz.
        public int FileId { get; set; }
    }
}
