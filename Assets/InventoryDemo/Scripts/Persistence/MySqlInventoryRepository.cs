using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using InventoryDemo.Data;
using MySqlConnector;

namespace InventoryDemo.Persistence
{
    public sealed class MySqlInventoryRepository
    {
        public const string ConnectionStringEnvironmentVariable =
            "LEARN_INVENTORY_DB_CONNECTION";

        private readonly string connectionString;

        private MySqlInventoryRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public static bool TryCreateFromEnvironment(
            out MySqlInventoryRepository repository,
            out string errorMessage)
        {
            repository = null;
            errorMessage = string.Empty;

            string value = Environment.GetEnvironmentVariable(
                ConnectionStringEnvironmentVariable);

            if (string.IsNullOrWhiteSpace(value))
            {
                try
                {
                    value = Environment.GetEnvironmentVariable(
                        ConnectionStringEnvironmentVariable,
                        EnvironmentVariableTarget.User);
                }
                catch (PlatformNotSupportedException)
                {
                    value = null;
                }
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                errorMessage =
                    $"Environment variable '{ConnectionStringEnvironmentVariable}' is not configured.";
                return false;
            }

            try
            {
                var builder = new MySqlConnectionStringBuilder(value);
                repository = new MySqlInventoryRepository(builder.ConnectionString);
                return true;
            }
            catch (ArgumentException exception)
            {
                errorMessage = $"The inventory database connection setting is invalid: {exception.Message}";
                return false;
            }
        }

        public async Task<InventoryLoadResult> LoadInventoryAsync(
            long playerId,
            CancellationToken cancellationToken)
        {
            if (playerId <= 0)
            {
                return InventoryLoadResult.Failure("The demo player ID must be positive.");
            }

            try
            {
                var items = new List<InventoryItemData>();
                await using var connection = new MySqlConnection(connectionString);
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                await using MySqlCommand command = connection.CreateCommand();
                command.CommandText =
                    "SELECT item_code, slot_index, quantity " +
                    "FROM player_inventory_items " +
                    "WHERE player_id = @playerId " +
                    "ORDER BY slot_index;";
                command.Parameters.Add("@playerId", MySqlDbType.Int64).Value = playerId;

                await using MySqlDataReader reader =
                    await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                int itemCodeOrdinal = reader.GetOrdinal("item_code");
                int slotIndexOrdinal = reader.GetOrdinal("slot_index");
                int quantityOrdinal = reader.GetOrdinal("quantity");

                while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    string itemCode = reader.GetString(itemCodeOrdinal);
                    int slotIndex = reader.GetInt32(slotIndexOrdinal);
                    int quantity = reader.GetInt32(quantityOrdinal);

                    if (!InventoryItemData.TryCreate(
                            itemCode,
                            slotIndex,
                            quantity,
                            out InventoryItemData item))
                    {
                        return InventoryLoadResult.Failure(
                            $"The database returned an invalid inventory row for slot {slotIndex}.");
                    }

                    items.Add(item);
                }

                return InventoryLoadResult.Success(items);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                return InventoryLoadResult.Failure(exception.Message);
            }
        }
    }
}
