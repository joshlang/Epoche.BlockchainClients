using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epoche.BlockchainClients.Near;
public class NearTransactionResult
{
    [JsonPropertyName("receipts_outcome")]
    public NearReceiptOutcome[] ReceiptsOutcome { get; init; } = default!;
}
