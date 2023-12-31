## C# Exercise
Dabbling with a console app


## Running the Program
Extract the contents of the .zip file 
```sh
  cd EventStreamProcessor
  dotnet run [command] [value]
  ```

### Exmaple Commands

To add transactions from a local JSON file
  ```sh
  dotnet run --read-file transactions.json
  ```

To add transactions directly from the terminal (requires a valid stringified JSON object or array)
  ```sh
  dotnet run --read-inline '{"Type": "Burn", "TokenId": “0x..."}'
  ```

To check the owner of an NFT
  ```sh
  dotnet run --nft [token_id]
  ```

To list NFTs owned by a wallet address
  ```sh
  dotnet run --wallet [wallet_address]
  ```

To reset the data stored in the local database
  ```sh
  dotnet run --reset
  ```
