namespace RedisExplorer;

using RedisExplorer.Models;

public partial class StartPage
{
    public List<RedisDatabaseConfiguration> Configs
    {
        get => ConnectionService.Configurations;
    }

    private ConnectionService ConnectionService { get; set; }

    public StartPage()
    {
        ConnectionService = new ConnectionService();

        InitializeComponent();

        BindingContext = this;


    }

    public async void HelpButtonClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Help", "Telegram: https://t.me/Kir1llLL", "OK");
    }



    private async void BrowserRedisOpenClicked(object sender, System.EventArgs e)
    {
        try
        {
            Uri uri = new Uri("https://redis.io/docs/");
            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception )
        {
            // An unexpected error occurred. No browser may be installed on the device.
        }
    }

    private async void BrowserDockerOpenClicked(object sender, System.EventArgs e)
    {
        try
        {
            Uri uri = new Uri("https://redis.io/docs/stack/get-started/install/docker/");
            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception)
        {
            // An unexpected error occurred. No browser may be installed on the device.
        }
    }

    private async void BrowserGitHubOpenClicked(object sender, System.EventArgs e)
    {
        try
        {
            Uri uri = new Uri("https://github.com/KirillNasyrov/RedisExplorer");
            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception)
        {
            // An unexpected error occurred. No browser may be installed on the device.
        }
    }



    private async void RedisConfigurationSelectedAsync(object sender, SelectionChangedEventArgs e)
    {
        if (TableOfConfigs.SelectedItem == null)
        {
            return;
        }

        try
        {
            var selectedConfig = (RedisDatabaseConfiguration)TableOfConfigs.SelectedItem;

            if (!ConnectionService.WasConnected(selectedConfig.DatabaseID))
            {
                var connection = await ConnectionService.CreateConnectionAsync(selectedConfig.DatabaseID);

                ConnectionService.AddNewConnection(selectedConfig.DatabaseID, connection);
            }

            ConnectionService.Configurations.Find(conf => conf.DatabaseID == selectedConfig.DatabaseID)
                .UpdateLastTimeConnection();
            TableOfConfigs.ItemsSource = new List<RedisDatabaseConfiguration>(Configs);
            ConnectionService.UpdateConfigsAsync();

            await Navigation.PushAsync(new RedisDatabasePage(ConnectionService.Connections[selectedConfig.DatabaseID], selectedConfig), false);
        }
        catch (ArgumentException ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Can not connect to database", "OK");
        }
        finally
        {
            TableOfConfigs.SelectedItem = null;
        }
    }


    private void RedisConfigurationRemoved(object sender, System.EventArgs e)
    {
        var button = sender as ImageButton;
        var currentConfig = button.CommandParameter as RedisDatabaseConfiguration;

        ConnectionService.Configurations.Remove(currentConfig);

        TableOfConfigs.ItemsSource = new List<RedisDatabaseConfiguration>(Configs);
        ConnectionService.UpdateConfigsAsync();
    }

    private void AddRedisDataBaseButtonClicked(object sender, System.EventArgs e)
    {
        GridOfConfigs.ColumnDefinitions.ElementAt(5).Width = new GridLength(2, GridUnitType.Star);
    }

    private void CancelAddingDataBaseButtonClicked(object sender, System.EventArgs e)
    {
        DataBaseNameEntry.Text = null;
        DataBaseHostEntry.Text = null;
        DataBasePortEntry.Text = null;
        DataBasePassswordEntry.Text = null;

        GridOfConfigs.ColumnDefinitions.ElementAt(5).Width = new GridLength(0, GridUnitType.Star);
    }

    private async void ApplyAddingDatabaseButtonClickedAsync(object sender, System.EventArgs e)
    {
        if (Configs.FindAll((config) => config.DatabaseID == DataBaseNameEntry.Text).Any())
        {
            await DisplayAlert("Error", "You already has database with such name", "OK");
        }
        else
        {
            var newConfig = new RedisDatabaseConfiguration(DataBaseNameEntry.Text, DataBaseHostEntry.Text,
                    DataBasePortEntry.Text, DataBasePassswordEntry.Text);
            try
            {
                ConnectionService.Configurations.Add(newConfig);

                var connection = await ConnectionService.CreateConnectionAsync(newConfig.DatabaseID);
                ConnectionService.AddNewConnection(newConfig.DatabaseID, connection);


                await Navigation.PushAsync(new RedisDatabasePage(connection, newConfig), false);

                TableOfConfigs.ItemsSource = new List<RedisDatabaseConfiguration>(Configs);
                ConnectionService.UpdateConfigsAsync();


                GridOfConfigs.ColumnDefinitions.ElementAt(5).Width = new GridLength(0, GridUnitType.Star);
            }
            catch (ArgumentException ex)
            {
                ConnectionService.Configurations.Remove(newConfig);
                await DisplayAlert("Error", ex.Message, "OK");
            }
            catch (Exception)
            {
                ConnectionService.Configurations.Remove(newConfig);
                await DisplayAlert("Error", "Can not connect to database", "OK");
            }
        }
    }

}