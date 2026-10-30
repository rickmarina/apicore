using com.rorisoft.excepcion;
using com.rorisoft.log;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Text;

namespace com.rorisoft.db
{
    public class MotorDBMSSQL : InterfaceDB, IDisposable
    {
        //Variables privadas
        private SqlDataReader? reader;
        private SqlCommand? comando;
        private SqlConnection? conexion;
        private string cadenaConexion;
        private SqlDataAdapter? adapter;
        private SqlTransaction? trans;


        public MotorDBMSSQL(string cadena)
        {
            this.cadenaConexion = cadena;
        }

        public void setCadenaConexion(string cad)
        {
            this.cadenaConexion = cad;
        }

        public void Connect()
        {
            try
            {
                if (this.conexion?.State != ConnectionState.Open)
                {
                    this.conexion = new SqlConnection(this.cadenaConexion);
                    this.conexion.Open();
                }
            }
            catch (SqlException ex)
            {
                throw new BBDDException(BBDDException.TipoError.Fatal, "Error al conectar a la DB: ", ex);
            }
        }

        //-- Gestión de transacciones --
        #region "Gestión de transacciones"
        public void beginTransaction(String transname)
        {
            this.trans = this.conexion?.BeginTransaction(IsolationLevel.ReadCommitted);
        }
        public void linkTransaction()
        {
            ArgumentNullException.ThrowIfNull(this.comando, "Comando no puede ser nulo. Debe establecerse antes de llamar a linkTransaction()");

            this.comando.Transaction = this.trans;
        }
        public void rollBackTransaction() { this.trans?.Rollback(); }
        public void commitTransaction() { this.trans?.Commit(); }
        #endregion

        //public static long getLastIdentity(InterfaceDB db)
        //{
        //    String query = "SELECT @@identity";
        //    db.setComando(query);
        //    long resultado = db.ejecutarScalar();
        //    return resultado;
        //}

        public DataSet Query(String q)
        {
            try
            {
                if (this.conexion?.State == ConnectionState.Closed) { Connect(); }
                this.comando = new SqlCommand(q, this.conexion);
                this.comando.CommandType = CommandType.Text;
                this.adapter = new SqlDataAdapter(this.comando);

                DataSet ds = new DataSet();
                this.adapter.Fill(ds);

                return ds;
            }
            catch (NullReferenceException ex) { throw new BBDDException(BBDDException.TipoError.Critica, "Dataset vacío", ex); }
            catch (SqlException exsql) { throw new BBDDException(BBDDException.TipoError.Fatal, "Error al conectar db", exsql); }
        }

        public DataSet Query()
        {

            ArgumentNullException.ThrowIfNull(this.comando, "Comando no puede ser nulo. Debe establecerse antes de llamar a Query()");
            try
            {
                if (this.conexion?.State == ConnectionState.Closed) { Connect(); }
                this.comando.CommandType = CommandType.Text;
                this.adapter = new SqlDataAdapter(this.comando);

                DataSet ds = new DataSet();
                this.adapter.Fill(ds);

                return ds;
            }
            catch (NullReferenceException ex) { throw new BBDDException(BBDDException.TipoError.Critica, "Dataset vacío", ex); }
            catch (SqlException exsql) { throw new BBDDException(BBDDException.TipoError.Fatal, "Error al conectar db", exsql); }
        }

        public void setComando(String q) { this.comando = new SqlCommand(q, this.conexion); }
        public void setComando(String q, long limite) { setComando(q + " limit " + limite.ToString() + ";"); }
        public void setComandoTimeOut(String q, int t)
        {
            ArgumentNullException.ThrowIfNull(this.comando, "Comando no puede ser nulo. Debe establecerse antes de llamar a setComandoTimeOut()");
            this.comando.CommandTimeout = t; setComando(q);
        }
        public void setTimeOut(int t)
        {
            ArgumentNullException.ThrowIfNull(this.comando, "Comando no puede ser nulo. Debe establecerse antes de llamar a setTimeOut()");
            this.comando.CommandTimeout = t;
        }
        public void clearParameters()
        {
            ArgumentNullException.ThrowIfNull(this.comando, "Comando no puede ser nulo. Debe establecerse antes de llamar a clearParameters()");
            this.comando.Parameters.Clear();
        }

        public void addParameter(String param, string? val)
        {
            if (val is null)
                this.comando?.Parameters.AddWithValue(param, DBNull.Value);
            else
                this.comando?.Parameters.AddWithValue(param, val);
        }
        public void addParameter(String param, DBNull val) { this.comando?.Parameters.AddWithValue(param, DBNull.Value); }
        public void addParameter(String param, int val) { this.comando?.Parameters.AddWithValue(param, val); }
        public void addParameter(String param, decimal val) { this.comando?.Parameters.AddWithValue(param, val); }
        public void addParameter(String param, double val) { this.comando?.Parameters.AddWithValue(param, val); }
        public void addParameter(String param, long val) { this.comando?.Parameters.AddWithValue(param, val); }
        public void addParameter(String param, bool val) { this.comando?.Parameters.AddWithValue(param, val); }
        public void addParameter(String param, DateTime val) { this.comando?.Parameters.AddWithValue(param, val); }
        public void addParameter(String param, Nullable<DateTime> val)
        {
            if (val == null)
                this.comando?.Parameters.AddWithValue(param, DBNull.Value);
            else
                this.comando?.Parameters.AddWithValue(param, val);
        }
        public void addParameter(String param, Nullable<Double> val)
        {
            if (val == null)
                this.comando?.Parameters.AddWithValue(param, DBNull.Value);
            else
                this.comando?.Parameters.AddWithValue(param, val);
        }
        public void addParameter(string param, byte[]? arr)
        {
            if (arr is null)
                this.comando?.Parameters.AddWithValue(param, DBNull.Value);
            else
                this.comando?.Parameters.AddWithValue(param, arr);
        }

        public void addParameterLike(String param, String value)
        {
            this.comando?.Parameters.AddWithValue(param, "%" + value + "%");
        }

        //Ejecuciones de consultas

        public int ejecutarQueryWithExitParameter()
        {
            SqlParameter param = new SqlParameter("@result", SqlDbType.Int);
            param.Direction = ParameterDirection.ReturnValue;
            this.comando?.Parameters.Add(param);
            int r = ejecutarQuery();
            return Convert.ToInt32(param.Value);
        }

        public int ejecutarQuery() { return this.comando?.ExecuteNonQuery() ?? 0; }
        public int ejecutarQuery(String q)
        {
            this.setComando(q);
            return this.comando?.ExecuteNonQuery() ?? 0;
        }

        public int ejecutarScalar()
        {
            int resultado = 0;
            try
            {
                resultado = Convert.ToInt32(this.comando?.ExecuteScalar());
            }
            catch (Exception) { resultado = 0; }
            return resultado;
        }
        public double ejecutarScalarDouble()
        {
            double resultado = 0;
            try
            {
                resultado = Convert.ToDouble(this.comando?.ExecuteScalar());
            }
            catch (Exception) { resultado = 0; }
            return resultado;
        }
        public decimal ejecutarScalarDecimal()
        {
            decimal resultado = 0;
            try
            {
                resultado = Convert.ToDecimal(this.comando?.ExecuteScalar());
            }
            catch (Exception) { resultado = 0; }
            return resultado;
        }
        public T ejecutarScalar<T>()
        {
            if (this.comando == null)
                throw new BBDDException("Command cannot be null. It must be set before calling ejecutarScalar<T>()");

            try
            {
                object? value = this.comando.ExecuteScalar();

                if (value is null || value == DBNull.Value)
                    return default!;

                if (value is T result)
                    return result;

                return (T)Convert.ChangeType(value, Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T));
            }
            catch (Exception)
            {
                throw;
            }

        }

        public T ejecutarScalarWithIdentity<T>()
        {
            ArgumentNullException.ThrowIfNull(this.comando, "Comando no puede ser nulo. Debe establecerse antes de llamar a ejecutarScalarWithIdentity()");

            if (this.comando?.CommandText.Trim().Length == 0)
                throw new BBDDException(BBDDException.TipoError.Critica, "No se puede ejecutar un comando que no existe", new Exception("No se puede ejecutar un comando que no existe"));

            if (this.comando!.CommandText.Trim()[this.comando.CommandText.Trim().Length - 1] == ';')
                this.comando.CommandText += " SELECT SCOPE_IDENTITY();";
            else
                this.comando.CommandText += "; SELECT SCOPE_IDENTITY();";

            return ejecutarScalar<T>();
        }

        public void procedimientoAlmacenado(String q)
        {
            ArgumentNullException.ThrowIfNull(this.comando, "Comando no puede ser nulo. Debe establecerse antes de llamar a procedimientoAlmacenado()");
            setComando(q); this.comando.CommandType = CommandType.StoredProcedure;
        }

        public bool IsProcedimientoAlmacenado()
        {
            return this.comando?.CommandType == CommandType.StoredProcedure;
        }
        public bool Read()
        {
            return this.reader?.Read() ?? false;
        }

        public IDataReader? getReader()
        {
            return this.reader;
        }

        public bool hasRows()
        {
            return this.reader?.HasRows ?? false;
        }

        public void QueryReader()
        {
            if (this.conexion?.State == ConnectionState.Closed) this.Connect();

            this.reader = this.comando?.ExecuteReader();
        }

        public String obtenerQueryConParametros()
        {
            ArgumentNullException.ThrowIfNull(this.comando, "Comando no puede ser nulo. Debe establecerse antes de llamar a obtenerQueryConParametros()");

            string? query = this.comando.CommandText;
            foreach (SqlParameter p in this.comando.Parameters)
            {
                query = query.Replace(p.ParameterName, p.Value.ToString());
            }
            return query;
        }

        public void TransformarParametrosMotor(string orig, string dest)
        {
            ArgumentNullException.ThrowIfNull(this.comando, "Comando no puede ser nulo. Debe establecerse antes de llamar a TransformarParametrosMotor()");

            foreach (SqlParameter param in this.comando.Parameters)
            {
                param.ParameterName = param.ParameterName.Replace(orig, dest);
            }
        }

        public void QueryReader(String q)
        {
            ArgumentNullException.ThrowIfNull(this.comando, "Comando no puede ser nulo. Debe establecerse antes de llamar a QueryReader(String q)");

            setComando(q);
            this.comando.CommandType = CommandType.Text;
            this.QueryReader();
        }
        public string queryResultString()
        {
            string r = string.Empty;
            QueryReader();

            if (this.reader is null)
                return r;

            if (this.reader.HasRows)
            {
                this.reader.Read();
                r = this.reader[0]?.ToString() ?? string.Empty;
            }
            else { r = ""; }
            this.reader.Close();
            return r;
        }

        public Dictionary<String, String> crearDiccionario(String q, String campoClave, String campoValor)
        {
            setComando(q);
            return crearDiccionario(campoClave, campoValor);
        }
        public Dictionary<String, String> crearDiccionario(String campoClave, String campoValor)
        {
            Dictionary<String, String> diccionario = [];
            try
            {
                DataSet datos = Query();
                foreach (DataRow item in datos.Tables[0].Rows)
                {
                    string? key = item[campoClave]?.ToString();
                    string? value = item[campoValor]?.ToString();

                    if (key is not null && value is not null)
                        _ = diccionario.TryAdd(key, value);
                }
            }
            catch (Exception ex)
            {
                throw new BBDDException(BBDDException.TipoError.Critica, "Error diccionario de claves", ex);
            }
            return diccionario;
        }


        public void Close()
        {
            if (this.reader != null)
                if (!this.reader.IsClosed)
                    this.reader.Close();

            if (this.conexion != null)
                this.conexion.Close();

        }
        public void CloseAndDispose()
        {
            Close();
            Dispose();
        }
        public void CloseReader()
        {
            this.reader?.Close();
        }

        public void Dispose()
        {
            this.comando?.Dispose();
            this.reader?.Dispose();
        }

        public int insertarLog(LogVO log)
        {
            int pk = 0;
            if (this.conexion != null)
            {
                StringBuilder query = new StringBuilder();
                query.Append("INSERT INTO Log (entidad, entidad_id, fecha, usuario, descripcion, prioridad) ");
                query.Append("VALUES (@entidad,@entidad_id,getdate(),@usuario, @descripcion, @prioridad); SELECT SCOPE_IDENTITY(); ");
                //mysql LAST_INSERT_ID(); 
                //mssql SELECT SCOPE_IDENTITY();
                this.Connect();
                this.setComando(query.ToString());

                addParameter("@entidad", log.entidad.ToString());
                addParameter("@entidad_id", log.entidadID);
                addParameter("@usuario", log.usuarioID);
                addParameter("@descripcion", log.descripcion);
                addParameter("@prioridad", log.prioridad.ToString());

                pk = ejecutarScalar();

            }
            else
            {
                throw new BBDDException("No se puede insertar log, la conexión es nula");
            }
            return pk;
        }

    }
}