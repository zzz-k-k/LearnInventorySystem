using System;
using System.Collections.Generic;
using InventoryDemo.Data;

namespace InventoryDemo.Persistence
{
    public sealed class InventoryLoadResult
    {
        private InventoryLoadResult(
            bool isSuccess,
            IReadOnlyList<InventoryItemData> items,
            string errorMessage)
        {
            IsSuccess = isSuccess;
            Items = items;
            ErrorMessage = errorMessage;
        }

        public bool IsSuccess { get; }
        public IReadOnlyList<InventoryItemData> Items { get; }
        public string ErrorMessage { get; }

        public static InventoryLoadResult Success(List<InventoryItemData> items)
        {
            return new InventoryLoadResult(
                true,
                items ?? throw new ArgumentNullException(nameof(items)),
                string.Empty);
        }

        public static InventoryLoadResult Failure(string errorMessage)
        {
            return new InventoryLoadResult(
                false,
                Array.Empty<InventoryItemData>(),
                string.IsNullOrWhiteSpace(errorMessage)
                    ? "Unknown inventory load error."
                    : errorMessage);
        }
    }
}
