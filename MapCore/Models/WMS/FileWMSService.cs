using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MapCore.Models.WMS
{
    public class FileWMSService : StreamReadingWMSService
    {
        private readonly string _path;

        public FileWMSService(string path)
        {
            _path = path;
        }

        protected override Task<Stream> GetWMSCapabilitiesStream()
        {
            var task = new Task<Stream>(() =>
            {
                var stream = new FileStream(_path, FileMode.Open);
                return stream;
            }
            );
            task.Start();
            return task;
        }
    }
}
