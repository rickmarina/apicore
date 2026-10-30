using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static com.rorisoft.log2.LogConstants;

namespace com.rorisoft.log2
{
    public interface ILogBehaviour
    {
        int log(ILogData data);
        int log(string msg, ILogData data); //si únicamente cambia el mensaje
    }

    public interface ILogData
    {
        public long id { get; set; }
        public string entidad { get; set; }
        public string entidadID { get; set; }
        public DateTime fecha { get; set; }
        public int usuarioID { get; set; }
        public string? descripcion { get; set; }
        public LOGLEVEL level { get; set; }
        public string? response { get; set; }
        public string? request { get; set; }
        public string? message { get; set; }
        public string? className { get; set; }
        public string? method { get; set; }
    }

    public static class LogConstants
    {
        public enum LOGENTITY
        {
            APLICACION,
            USUARIO,
            EMAIL,
            BBDD
        }
        public enum LOGLEVEL
        {
            DEBUG,
            INFO,
            WARNING,
            ERROR, 
            FATAL
        }

    }
}
