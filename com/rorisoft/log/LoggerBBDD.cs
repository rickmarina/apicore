using com.rorisoft.db;
using System.Text;

namespace com.rorisoft.log
{
    public class LoggerBBDD<T> : Logger<T> where T: ILogData, new()
    {

        private readonly InterfaceDB? _db = null;

        public LoggerBBDD(InterfaceDB motordb)
        {
            this._db = motordb;
        }

        public override int executeLog()
        {
            int pk = 0;
            if (this._db != null) { 
                StringBuilder query = new StringBuilder();
                query.Append("INSERT INTO Log (entidad, entidad_id, fecha, usuario, descripcion, prioridad) ");
                query.Append("VALUES (@entidad,@entidad_id,CURRENT_TIMESTAMP,@usuario, @descripcion, @prioridad); ");

                using (this._db) { 
                    if (this._db is MotorDBMSSQL)
                    {
                        query.Append("SELECT SCOPE_IDENTITY(); ");
                    }
                    else if (this._db is MotorDBMysql)
                    {
                        query.Append("select LAST_INSERT_ID(); ");
                    }

                    this._db.Connect();
                    this._db.setComando(query.ToString());

                    this._db.addParameter("@entidad", this._info.entidad);
                    this._db.addParameter("@entidad_id", this._info.entidadID);
                    this._db.addParameter("@usuario", this._info.usuarioID);
                    this._db.addParameter("@descripcion", this._info.descripcion ?? string.Empty);
                    this._db.addParameter("@prioridad", this._info.prioridad.ToString());

                    //pk = ejecutarScalar();
                    pk = this._db.ejecutarQuery();
                }
            }
            return pk;
        }

        public override int executeLog(string msg)
        {
            this._info.message = msg;
            return executeLog();
        }
    }
}
