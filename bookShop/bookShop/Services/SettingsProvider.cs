namespace server.Services {

    public interface ISettingsProvider {
        T GetValue<T>(string name);
    }

    public class SettingsProvider : ISettingsProvider {
        private readonly IConfiguration configuration;


        public SettingsProvider(IConfiguration configuration) {
            this.configuration = configuration;
        }

        public T GetValue<T>(string name) {
            return configuration.GetValue<T>(name);
        }
    }
}
