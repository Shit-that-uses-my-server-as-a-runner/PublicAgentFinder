A problem for EffectiveMobile company
Api must posess two features:
1. Loading data about publicity agencies from a file
2. Checking, which agencies perform in the specified location

Both actions are resolved by the same controller.
The first is to be found at Publicity/load It is a POST endpoint
To use it, you must provide a file in the body of the request (header form/multipart is automatically assigned by Postman)
Response is 200 with no additional information in case of success, 415 in case of unsupported media type (attempt to load anything but a file), 500 in case of wrong format or internal error
The second is situated at Publicity/getAgent/?location=...where location is a query string where you specify the location you want to find publicity agency for, like /ru/len/spb, /fr/idf/par, etc.
It returns 200 with an array of agencies found in case of success, a 204 if no such agencies were found, a 400 if the location specified is of wrong format

To be launched throug Visual Studio

Postman test requests collection: https://www.postman.com/sergearmstrong/public-workspace/collection/e1xauts/publicityapp?action=share&creator=42929010
