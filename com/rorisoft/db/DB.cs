using com.rorisoft.excepcion;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using com.rorisoft.log;

namespace com.rorisoft.db
{

    //TODO: Implementar un mapper 

    public class DB : IDisposable, ILogable<ILogData>, IDBContext
    {
        private InterfaceDB? motordb = null;
        private ILogable<LogVO>? _logger = null;

        private static int LIMIT_TRACE_QUERY = 75;
        private const Boolean DEBUG_QUERY = false;


        public DB(InterfaceDB motor, ILogable<ILogData>? logger = null)
        {
            this.motordb = motor;
            if (logger == null) //Por defecto creamos un logger de tipo Base de datos
                this._logger = new LoggerBBDD<LogVO>(this.motordb);

        }

        public ILogable<ILogData>? getLogger()
        {
            return (ILogable<ILogData>?)this._logger;
        }

        public void Connect()
        {
            conectar();
        }

        public void conectar()
        {
            if (this.motordb != null)
            {
                motordb.Connect();
            }
        }
        public void setCadenaConexion(string cad)
        {
            if (this.motordb != null)
            {
                motordb.setCadenaConexion(cad);
            }
        }

        public void setComando(string q)
        {
            if (this.motordb != null)
            {
                motordb.setComando(q);
            }
        }

        public void setProcedimientoAlmacenado(string sp)
        {
            if (this.motordb != null)
            {
                motordb.procedimientoAlmacenado(sp);
            }
        }

        public bool IsProcedimientoAlmacenado()
        {
            return this.motordb?.IsProcedimientoAlmacenado() ?? false;
        }

        public void setTimeOut(int t)
        {
            if (this.motordb != null)
            {
                motordb.setTimeOut(t);
            }
        }
        public void addParameter(String param, String val)
        {
            if (this.motordb != null)
            {
                this.motordb.addParameter(param, val);
            }
        }

        public void addParameter(string param, byte[]? arr)
        {
            if (this.motordb == null) 
                return;
            
            if (arr is null)
                this.motordb?.addParameter(param, DBNull.Value);
            else
                this.motordb?.addParameter(param, arr);
        }

        public void addParameterLike(string param, string val)
        {
            if (this.motordb != null)
            {
                this.motordb.addParameterLike(param, val);
            }
        }
        public void addParameter(String param, bool val)
        {
            if (this.motordb != null)
            {
                this.motordb.addParameter(param, val);
            }
        }
        public void addParameter(String param, DateTime val)
        {
            if (this.motordb != null)
            {
                this.motordb.addParameter(param, val);
            }
        }
        public void addParameter(String param, DateTime? val)
        {
            if (this.motordb != null)
            {
                this.motordb.addParameter(param, val);
            }
        }
        public void addParameter(String param, long val)
        {
            if (this.motordb != null)
            {
                this.motordb.addParameter(param, val);
            }
        }

        public void addParamter(string param, int val)
        {
            if (this.motordb != null)
            {
                this.motordb.addParameter(param, val);
            }
        }

        public void addParameterOptional(string param, int? val)
        {
            if (val.HasValue)
                this.motordb?.addParameter(param, val.Value);
            else
                this.motordb?.addParameter(param, DBNull.Value);
        }

        public void addParameterOptional(string param, decimal? val)
        {
            if (val.HasValue)
                this.motordb?.addParameter(param, val.Value);
            else
                this.motordb?.addParameter(param, DBNull.Value);
        }

        public void addParameterOptional(string param, bool? val)
        {
            if (val.HasValue)
                this.motordb?.addParameter(param, val.Value);
            else
                this.motordb?.addParameter(param, DBNull.Value);
        }

        public void addParameter(string param, DBNull val)
        {
            if (this.motordb != null)
            {
                this.motordb.addParameter(param, val);
            }
        }

        public void addParameter(String param, decimal val)
        {
            if (this.motordb != null)
            {
                this.motordb.addParameter(param, val);
            }
        }

        public void addParameter(String param, float val)
        {
            if (this.motordb != null)
            {
                this.motordb.addParameter(param, val);
            }
        }

        public void addParameter(string param, double val)
        {
            if (this.motordb != null)
                this.motordb.addParameter(param, val);
        }


        public void TransformarParametrosMotor(string orig, string dest)
        {
            if (this.motordb != null)
            {
                this.motordb.TransformarParametrosMotor(orig, dest);
            }
        }

        public void clearParameters()
        {
            if (this.motordb != null)
            {
                motordb.clearParameters();
            }
        }

        /// <summary>
        /// Ejecuta un comando SQL contra la base de datos
        /// En caso de que el motor sea mysql y sea un procedimiento almacenado, se reemplazan los parametros @ -> _
        /// </summary>
        /// <returns></returns>
        public int ejecutarQuery()
        {
            int res = 0;
            if (this.motordb != null)
            {
                if ((this.motordb is MotorDBMysql) && (this.motordb.IsProcedimientoAlmacenado()))
                {
                    this.motordb.TransformarParametrosMotor("@", "_");
                }
                res = motordb.ejecutarQuery();
            }
            return res;
        }
        public int ejecutarScalar()
        {
            int res = 0;
            if (this.motordb != null)
            {
                res = motordb.ejecutarScalar();
            }
            return res;
        }

        public T? ejecutarScalar<T>()
        {
            if (this.motordb != null)
            {
                return this.motordb.ejecutarScalar<T>();
            }

            return default;
        }

        /// <summary>
        /// Ejecuta y amplia la consulta para ejecutar el último registro insertado en la tabla
        /// </summary>
        /// <typeparam name="T">tipo de dato devuelvo ej, long</typeparam>
        /// <returns></returns>
        public T? ejecutarScalarWithIdentity<T>()
        {
            if (this.motordb != null)
            {
                return this.motordb.ejecutarScalarWithIdentity<T>();
            }

            return default;
        }

        public string queryResultString()
        {
            string r = String.Empty;
            if (this.motordb != null)
            {
                r = motordb.queryResultString();
            }
            return r;
        }
        public void queryReader()
        {
            if (this.motordb != null)
            {

                try
                {
                    Stopwatch delta = new Stopwatch();
                    delta.Start();

                    if ((this.motordb is MotorDBMysql) && (this.motordb.IsProcedimientoAlmacenado()))
                    {
                        this.motordb.TransformarParametrosMotor("@", "_");
                    }

                    motordb.QueryReader();
                    delta.Stop();

                    if (delta.ElapsedMilliseconds > LIMIT_TRACE_QUERY || DEBUG_QUERY)
                    {
                        Debug.WriteLine(String.Format("[Query {0} msecs]: {1}", delta.ElapsedMilliseconds, motordb.obtenerQueryConParametros()));
                    }

                }
                catch (NullReferenceException nullex)
                {
                    throw new BBDDException(BBDDException.TipoError.Fatal, "Conexión null no se puede ejecutar el reader", nullex);
                }
                catch (SqlException sqlex)
                {
                    throw new BBDDException(BBDDException.TipoError.Fatal, "Error en la llamada SQL:" + obtenerQueryParametrizada(), sqlex);
                }

            }
        }

        public DataSet? querySet()
        {
            DataSet? ds = null;
            if (this.motordb != null)
            {
                ds = this.motordb.Query();
            }
            return ds;
        }

        private string obtenerQueryParametrizada()
        {
            return this.motordb?.obtenerQueryConParametros() ?? "";
        }

        public bool canRead()
        {
            bool can = false;
            if (this.motordb != null)
            {
                can = this.motordb.Read();
            }
            return can;
        }

        public IDataReader? getReader()
        {
            return this.motordb?.getReader();
        }

        public bool HasRows()
        {
            return this.motordb?.hasRows() ?? false;
        }

        public IDataReader? getReader(DBFactory.TIPODB tipo)
        {
            if (tipo == DBFactory.TIPODB.mysql)
            {
                return (MySqlDataReader?)this.motordb?.getReader();
            }

            if (tipo == DBFactory.TIPODB.sql_server)
            {
                return (SqlDataReader?)this.motordb?.getReader();
            }

            return null;
        }

        public void closeReader()
        {
            if (this.motordb != null)
                this.motordb.CloseReader();
        }

        /// <summary>
        /// Permite obtener la consulta con los parámetros reemplazados por los valores 
        /// </summary>
        /// <returns></returns>
        public String obtenerTrazaQuery()
        {
            return this.motordb?.obtenerQueryConParametros() ?? string.Empty;
        }

        public void close()
        {
            if (this.motordb != null)
            {
                motordb.Close();
            }
        }

        /// <summary>
        /// Cierra Reader y Conexión
        /// </summary>
        public void closeAndDispose()
        {
            if (this.motordb != null)
            {
                motordb.CloseAndDispose();
            }
        }
        //public int insertarLog(ILogData log)
        //{
        //    int pk = 0;
        //    if (this.motordb != null)
        //    {
        //         pk = this.motordb.insertarLog(log);

        //    }
        //    return pk;
        //}

        public void Dispose()
        {
            closeAndDispose();
        }

        public int executeLog()
        {
            return _logger?.executeLog() ?? 0;
        }

        public int executeLog(string msg)
        {
            return _logger?.executeLog(msg) ?? 0;
        }

        public void setClass(string className)
        {
            _logger?.setClass(className);
        }

        public void setMethod(string methodName)
        {
            _logger?.setMethod(methodName);
        }

        public void setLogData(ILogData log)
        {
            _logger?.setLogData((LogVO)log);
        }
    }

    //TODO: Implementar un mapper automático 
    /*
     * public static void MapDataReaderToObject<T>(this SqlDataReader dataReader, T newObject)
    {
        if (newObject == null) throw new ArgumentNullException(nameof(newObject));

        // Fast Member Usage
        var objectMemberAccessor = TypeAccessor.Create(newObject.GetType());
        var propertiesHashSet =
                objectMemberAccessor
                .GetMembers()
                .Select(mp => mp.Name)
                .ToHashSet(StringComparer.InvariantCultureIgnoreCase);

        for (int i = 0; i < dataReader.FieldCount; i++)
        {
            var name = propertiesHashSet.FirstOrDefault(a => a.Equals(dataReader.GetName(i), StringComparison.InvariantCultureIgnoreCase));
            if (!String.IsNullOrEmpty(name))
            {
                //Attention! if you are getting errors here, then double check that your model and sql have matching types for the field name.
                //Check api.log for error message!
                objectMemberAccessor[newObject, name]
                    = dataReader.IsDBNull(i) ? null : dataReader.GetValue(i);
            }
        }


           public async Task<List<T>> ExecuteReaderAsync<T>(string storedProcedureName, SqlParameter[] sqlParameters = null) where T : class, new()
        {
            var newListObject = new List<T>();

            using (var conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand sqlCommand = GetSqlCommand(conn, storedProcedureName, sqlParameters))
                {
                    await conn.OpenAsync();
                    using (var dataReader = await sqlCommand.ExecuteReaderAsync(CommandBehavior.Default))
                    {
                        if (dataReader.HasRows)
                        {
                            while (await dataReader.ReadAsync())
                            {
                                var newObject = new T();
                                dataReader.MapDataReaderToObject(newObject);
                                newListObject.Add(newObject);
                            }
                        }
                    }
                }
            }

            return newListObject;
        }

        */


}


