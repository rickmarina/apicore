namespace com.rorisoft.log2
{
    public class LogData : ILogData
    {

        public LogData()
        {
            this.fecha = DateTime.Now;
            this.level = LogConstants.LOGLEVEL.INFO;
            this.entidad = nameof(LogConstants.LOGENTITY.APLICACION);
            this.entidadID = "0";
            this.usuarioID = 0;
        }
        public long id { get; set; }
        public string entidad { get; set; }
        public string entidadID { get; set; }
        public DateTime fecha { get; set; }
        public int usuarioID { get; set; }
        public string? descripcion { get; set; }
        public LogConstants.LOGLEVEL level { get; set; }
        public string? response { get; set; }
        public string? request { get; set; }
        public string? message { get; set; }
        public string? className { get; set; }
        public string? method { get; set; }
    }
}
