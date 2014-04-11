namespace Excho.Market

type Money = {
  Amount : double
  Currency : string
  Source : IInventory<Money>
}

