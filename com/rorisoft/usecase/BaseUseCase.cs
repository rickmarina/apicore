using com.rorisoft.environment;
using System.Text.Json;

namespace com.rorisoft.usecase
{

    public abstract class BaseUseCase //: AbstractConfigurationUseCase
    {
        //Variables de configuración para la lógica
        public static string CONFIG_URLGOV { get; private set; }
        public static string CONFIG_ENVIRONMENT { get; private set; }
        public static string CONFIG_CONNECTION { get; private set; }
        public static bool CONFIG_LOGBBDD { get; private set; } = false;
        public static string STORAGE_CONNETION { get; private set; }
        public static string STORAGE_CONTAINER { get; private set; }

        static BaseUseCase()
        {
            CONFIG_URLGOV = EnvironmentService.getSingleton(EnvironmentService.Environment_Type.APPSETTINGS_LOCAL).GetOtherConfig("url_es") ?? string.Empty;
            CONFIG_ENVIRONMENT = EnvironmentService.getSingleton(EnvironmentService.Environment_Type.APPSETTINGS_LOCAL).GetEnvironment ?? string.Empty;
            CONFIG_CONNECTION = EnvironmentService.getSingleton(EnvironmentService.Environment_Type.APPSETTINGS_LOCAL).GetConnection ?? string.Empty;
            CONFIG_LOGBBDD = EnvironmentService.getSingleton(EnvironmentService.Environment_Type.APPSETTINGS_LOCAL).GetLogBBDD;

            STORAGE_CONNETION = EnvironmentService.getSingleton(EnvironmentService.Environment_Type.APPSETTINGS_LOCAL).GetOtherConfig("Azure:storage") ?? string.Empty;
            STORAGE_CONTAINER = EnvironmentService.getSingleton(EnvironmentService.Environment_Type.APPSETTINGS_LOCAL).GetOtherConfig("Azure:container") ?? string.Empty;
        }


        public static List<KeyValuePair<string, string>> obtenerConfiguracion()
        {
            try
            {
                return EnvironmentService.getSingleton(EnvironmentService.Environment_Type.APPSETTINGS_LOCAL).obtenerConfiguracion();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string GetOtherConfigValue(string key)
        {
            try
            {
                return EnvironmentService.getSingleton(EnvironmentService.Environment_Type.APPSETTINGS_LOCAL).GetOtherConfig(key) ?? string.Empty;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<TResult> TransformModel<TParam, TResult>(List<TParam> listado)
        {
            var json = JsonSerializer.Serialize(listado);
            List<TResult>? resultado = JsonSerializer.Deserialize<List<TResult>>(json);
            return resultado ?? [];
        }

        public static TResult? TransformModelInfoEstacion<TParam, TResult>(TParam listado)
        {
            var json = JsonSerializer.Serialize(listado);
            TResult? resultado = JsonSerializer.Deserialize<TResult>(json);
            return resultado;
        }

    }

}
