namespace Intent.MetadataSynchronizer.OpenApi.CLI.Tests
{
    partial class ItShouldWork
    {
        private readonly string Test1Json = """
                       {
                         "openapi": "3.0.1",
                         "info": {
                           "title": "[DEV] TradeArt API",
                           "description": "TradeArt external API",
                           "version": "v1"
                         },
                         "paths": {
                           "/api/Account/login": {
                             "post": {
                               "tags": [
                                 "Account"
                               ],
                               "summary": "Log in to the system.",
                               "operationId": "Login",
                               "requestBody": {
                                 "description": "Credentials of client.",
                                 "content": {
                                   "application/json": {
                                     "schema": {
                                       "allOf": [
                                         {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Authentication.LoginModel"
                                         }
                                       ]
                                     }
                                   },
                                   "text/json": {
                                     "schema": {
                                       "allOf": [
                                         {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Authentication.LoginModel"
                                         }
                                       ]
                                     }
                                   },
                                   "application/*+json": {
                                     "schema": {
                                       "allOf": [
                                         {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Authentication.LoginModel"
                                         }
                                       ]
                                     }
                                   }
                                 }
                               },
                               "responses": {
                                 "200": {
                                   "description": "OK",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "type": "string"
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "type": "string"
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "type": "string"
                                       }
                                     }
                                   }
                                 },
                                 "429": {
                                   "description": "Too many requests"
                                 }
                               }
                             }
                           },
                           "/api/players/{playerId}/Bets/{requestId}": {
                             "get": {
                               "tags": [
                                 "Bets"
                               ],
                               "summary": "Gets data of the specific existing bet.",
                               "operationId": "Get bet",
                               "parameters": [
                                 {
                                   "name": "playerId",
                                   "in": "path",
                                   "description": "Id of player which placed a bet.",
                                   "required": true,
                                   "schema": {
                                     "type": "string"
                                   }
                                 },
                                 {
                                   "name": "requestId",
                                   "in": "path",
                                   "description": "Bet request Id.",
                                   "required": true,
                                   "schema": {
                                     "type": "string"
                                   }
                                 }
                               ],
                               "responses": {
                                 "200": {
                                   "description": "Returns information about the requested bet.",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.BetPlacement.BetResultViewModel"
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.BetPlacement.BetResultViewModel"
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.BetPlacement.BetResultViewModel"
                                       }
                                     }
                                   }
                                 },
                                 "400": {
                                   "description": "Request arguments are not valid.",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     }
                                   }
                                 },
                                 "500": {
                                   "description": "Unexpected error occurred."
                                 },
                                 "401": {
                                   "description": "Unauthorized"
                                 },
                                 "403": {
                                   "description": "Forbidden"
                                 },
                                 "429": {
                                   "description": "Too many requests"
                                 }
                               },
                               "security": [
                                 {
                                   "Bearer": [ ]
                                 }
                               ]
                             }
                           },
                           "/api/players/{playerId}/Bets/{requestId}/settlements": {
                             "get": {
                               "tags": [
                                 "Bets"
                               ],
                               "summary": "Gets settlement info of the specific existing bet.",
                               "operationId": "Get bet settlements",
                               "parameters": [
                                 {
                                   "name": "playerId",
                                   "in": "path",
                                   "description": "Id of player which placed a bet.",
                                   "required": true,
                                   "schema": {
                                     "type": "string"
                                   }
                                 },
                                 {
                                   "name": "requestId",
                                   "in": "path",
                                   "description": "Bet request Id.",
                                   "required": true,
                                   "schema": {
                                     "type": "string"
                                   }
                                 }
                               ],
                               "responses": {
                                 "200": {
                                   "description": "Returns information about settlement of the requested bet.",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Models.Settlement.SettledBetViewModelV2"
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Models.Settlement.SettledBetViewModelV2"
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Models.Settlement.SettledBetViewModelV2"
                                       }
                                     }
                                   }
                                 },
                                 "400": {
                                   "description": "Request arguments are not valid.",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     }
                                   }
                                 },
                                 "500": {
                                   "description": "Unexpected error occurred."
                                 },
                                 "401": {
                                   "description": "Unauthorized"
                                 },
                                 "403": {
                                   "description": "Forbidden"
                                 },
                                 "429": {
                                   "description": "Too many requests"
                                 }
                               },
                               "security": [
                                 {
                                   "Bearer": [ ]
                                 }
                               ]
                             }
                           },
                           "/api/players/{playerId}/Bets/{requestId}/cashOut": {
                             "post": {
                               "tags": [
                                 "Bets"
                               ],
                               "summary": "Registers bet cashout with specific amount.",
                               "operationId": "CashOut bet",
                               "parameters": [
                                 {
                                   "name": "playerId",
                                   "in": "path",
                                   "description": "Id of player which placed a bet.",
                                   "required": true,
                                   "schema": {
                                     "type": "string"
                                   }
                                 },
                                 {
                                   "name": "requestId",
                                   "in": "path",
                                   "description": "Bet request Id.",
                                   "required": true,
                                   "schema": {
                                     "type": "string"
                                   }
                                 }
                               ],
                               "requestBody": {
                                 "description": "CashOut amount.",
                                 "content": {
                                   "application/json": {
                                     "schema": {
                                       "allOf": [
                                         {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Models.Settlement.CashOutViewModel"
                                         }
                                       ],
                                       "description": "CashOut request model"
                                     }
                                   },
                                   "text/json": {
                                     "schema": {
                                       "allOf": [
                                         {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Models.Settlement.CashOutViewModel"
                                         }
                                       ],
                                       "description": "CashOut request model"
                                     }
                                   },
                                   "application/*+json": {
                                     "schema": {
                                       "allOf": [
                                         {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Models.Settlement.CashOutViewModel"
                                         }
                                       ],
                                       "description": "CashOut request model"
                                     }
                                   }
                                 }
                               },
                               "responses": {
                                 "204": {
                                   "description": "Cashout registered successfully."
                                 },
                                 "400": {
                                   "description": "Request arguments are not valid.",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     }
                                   }
                                 },
                                 "404": {
                                   "description": "Bet not found for player.",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     }
                                   }
                                 },
                                 "500": {
                                   "description": "Unexpected error occurred."
                                 },
                                 "401": {
                                   "description": "Unauthorized"
                                 },
                                 "403": {
                                   "description": "Forbidden"
                                 },
                                 "429": {
                                   "description": "Too many requests"
                                 }
                               },
                               "security": [
                                 {
                                   "Bearer": [ ]
                                 }
                               ]
                             }
                           },
                           "/api/players/{playerId}/Bets": {
                             "put": {
                               "tags": [
                                 "Bets"
                               ],
                               "summary": "Places bet from specific player.",
                               "operationId": "Place bet",
                               "parameters": [
                                 {
                                   "name": "playerId",
                                   "in": "path",
                                   "description": "Id of player which places a bet.",
                                   "required": true,
                                   "schema": {
                                     "type": "string"
                                   }
                                 }
                               ],
                               "requestBody": {
                                 "description": "Bet request data.",
                                 "content": {
                                   "application/json": {
                                     "schema": {
                                       "allOf": [
                                         {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.BetPlacement.BetRequestViewModel"
                                         }
                                       ],
                                       "description": "Represents bet request."
                                     }
                                   },
                                   "text/json": {
                                     "schema": {
                                       "allOf": [
                                         {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.BetPlacement.BetRequestViewModel"
                                         }
                                       ],
                                       "description": "Represents bet request."
                                     }
                                   },
                                   "application/*+json": {
                                     "schema": {
                                       "allOf": [
                                         {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.BetPlacement.BetRequestViewModel"
                                         }
                                       ],
                                       "description": "Represents bet request."
                                     }
                                   }
                                 }
                               },
                               "responses": {
                                 "200": {
                                   "description": "Returns results of bet placement request.",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.BetPlacement.BetResultViewModel"
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.BetPlacement.BetResultViewModel"
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.BetPlacement.BetResultViewModel"
                                       }
                                     }
                                   }
                                 },
                                 "400": {
                                   "description": "Request arguments are not valid.",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     }
                                   }
                                 },
                                 "500": {
                                   "description": "Unexpected error occurred."
                                 },
                                 "401": {
                                   "description": "Unauthorized"
                                 },
                                 "403": {
                                   "description": "Forbidden"
                                 },
                                 "429": {
                                   "description": "Too many requests"
                                 }
                               },
                               "security": [
                                 {
                                   "Bearer": [ ]
                                 }
                               ]
                             }
                           },
                           "/api/players/{playerId}/Bets/max": {
                             "post": {
                               "tags": [
                                 "Bets"
                               ],
                               "summary": "Gets max possible bet size for the specific player.",
                               "operationId": "Get max bet amount",
                               "parameters": [
                                 {
                                   "name": "playerId",
                                   "in": "path",
                                   "description": "Id of player which places a bet.",
                                   "required": true,
                                   "schema": {
                                     "type": "string"
                                   }
                                 }
                               ],
                               "requestBody": {
                                 "description": "Bet request data.",
                                 "content": {
                                   "application/json": {
                                     "schema": {
                                       "allOf": [
                                         {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.BetPlacement.BetRequestViewModel"
                                         }
                                       ],
                                       "description": "Represents bet request."
                                     }
                                   },
                                   "text/json": {
                                     "schema": {
                                       "allOf": [
                                         {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.BetPlacement.BetRequestViewModel"
                                         }
                                       ],
                                       "description": "Represents bet request."
                                     }
                                   },
                                   "application/*+json": {
                                     "schema": {
                                       "allOf": [
                                         {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.BetPlacement.BetRequestViewModel"
                                         }
                                       ],
                                       "description": "Represents bet request."
                                     }
                                   }
                                 }
                               },
                               "responses": {
                                 "200": {
                                   "description": "Returns results of max bet calculation.",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.BetPlacement.MaxBetResultViewModel"
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.BetPlacement.MaxBetResultViewModel"
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.BetPlacement.MaxBetResultViewModel"
                                       }
                                     }
                                   }
                                 },
                                 "400": {
                                   "description": "Request arguments are not valid.",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     }
                                   }
                                 },
                                 "500": {
                                   "description": "Unexpected error occurred."
                                 },
                                 "401": {
                                   "description": "Unauthorized"
                                 },
                                 "403": {
                                   "description": "Forbidden"
                                 },
                                 "429": {
                                   "description": "Too many requests"
                                 }
                               },
                               "security": [
                                 {
                                   "Bearer": [ ]
                                 }
                               ]
                             }
                           },
                           "/api/players/{playerId}/Bets/{requestId}/cancel": {
                             "post": {
                               "tags": [
                                 "Bets"
                               ],
                               "summary": "Cancels existing bet with specific reason.",
                               "operationId": "Cancel bet",
                               "parameters": [
                                 {
                                   "name": "playerId",
                                   "in": "path",
                                   "description": "Id of player which placed a bet.",
                                   "required": true,
                                   "schema": {
                                     "type": "string"
                                   }
                                 },
                                 {
                                   "name": "requestId",
                                   "in": "path",
                                   "description": "Bet request id.",
                                   "required": true,
                                   "schema": {
                                     "type": "string"
                                   }
                                 }
                               ],
                               "requestBody": {
                                 "description": "Cancellation data.",
                                 "content": {
                                   "application/json": {
                                     "schema": {
                                       "allOf": [
                                         {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.BetPlacement.CancelBetViewModel"
                                         }
                                       ]
                                     }
                                   },
                                   "text/json": {
                                     "schema": {
                                       "allOf": [
                                         {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.BetPlacement.CancelBetViewModel"
                                         }
                                       ]
                                     }
                                   },
                                   "application/*+json": {
                                     "schema": {
                                       "allOf": [
                                         {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.BetPlacement.CancelBetViewModel"
                                         }
                                       ]
                                     }
                                   }
                                 }
                               },
                               "responses": {
                                 "200": {
                                   "description": "Cancellation request sent."
                                 },
                                 "400": {
                                   "description": "Request arguments are not valid.",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     }
                                   }
                                 },
                                 "500": {
                                   "description": "Unexpected error occurred."
                                 },
                                 "401": {
                                   "description": "Unauthorized"
                                 },
                                 "403": {
                                   "description": "Forbidden"
                                 },
                                 "429": {
                                   "description": "Too many requests"
                                 }
                               },
                               "security": [
                                 {
                                   "Bearer": [ ]
                                 }
                               ]
                             }
                           },
                           "/api/Dictionary/reject-reasons": {
                             "get": {
                               "tags": [
                                 "Dictionary"
                               ],
                               "summary": "Gets possible reject reasons that can exist.",
                               "operationId": "Reject reasons",
                               "responses": {
                                 "200": {
                                   "description": "OK"
                                 },
                                 "401": {
                                   "description": "Unauthorized"
                                 },
                                 "403": {
                                   "description": "Forbidden"
                                 },
                                 "429": {
                                   "description": "Too many requests"
                                 }
                               },
                               "security": [
                                 {
                                   "Bearer": [ ]
                                 }
                               ]
                             }
                           },
                           "/api/events/schedule": {
                             "get": {
                               "tags": [
                                 "Feed"
                               ],
                               "summary": "Gets list of event with specified UTC start date.",
                               "operationId": "Get scheduled events",
                               "parameters": [
                                 {
                                   "name": "startDate",
                                   "in": "query",
                                   "description": "Start date UTC in yyyy-MM-dd format.",
                                   "required": true,
                                   "schema": {
                                     "type": "string",
                                     "format": "date-time"
                                   }
                                 },
                                 {
                                   "name": "sportId",
                                   "in": "query",
                                   "description": "Optional TradeArt sport id.",
                                   "schema": {
                                     "type": "integer",
                                     "format": "int64"
                                   }
                                 }
                               ],
                               "responses": {
                                 "200": {
                                   "description": "OK",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.EventInfoViewModelV2"
                                         }
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.EventInfoViewModelV2"
                                         }
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.EventInfoViewModelV2"
                                         }
                                       }
                                     }
                                   }
                                 },
                                 "400": {
                                   "description": "Bad Request",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     }
                                   }
                                 },
                                 "500": {
                                   "description": "Internal Server Error"
                                 },
                                 "401": {
                                   "description": "Unauthorized"
                                 },
                                 "403": {
                                   "description": "Forbidden"
                                 },
                                 "429": {
                                   "description": "Too many requests"
                                 }
                               },
                               "deprecated": true,
                               "security": [
                                 {
                                   "Bearer": [ ]
                                 }
                               ]
                             }
                           },
                           "/api/events/schedule/v2": {
                             "post": {
                               "tags": [
                                 "Feed"
                               ],
                               "summary": "Gets list of non-outright event with specified UTC start date.",
                               "requestBody": {
                                 "description": "Request",
                                 "content": {
                                   "application/json": {
                                     "schema": {
                                       "allOf": [
                                         {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.FeedFactory.QueryTradingEventModel"
                                         }
                                       ]
                                     }
                                   },
                                   "text/json": {
                                     "schema": {
                                       "allOf": [
                                         {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.FeedFactory.QueryTradingEventModel"
                                         }
                                       ]
                                     }
                                   },
                                   "application/*+json": {
                                     "schema": {
                                       "allOf": [
                                         {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.FeedFactory.QueryTradingEventModel"
                                         }
                                       ]
                                     }
                                   }
                                 },
                                 "required": true
                               },
                               "responses": {
                                 "400": {
                                   "description": "Bad Request",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     }
                                   }
                                 },
                                 "500": {
                                   "description": "Internal Server Error"
                                 },
                                 "200": {
                                   "description": "OK",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.EventInfoViewModelV2"
                                         }
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.EventInfoViewModelV2"
                                         }
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.EventInfoViewModelV2"
                                         }
                                       }
                                     }
                                   }
                                 },
                                 "401": {
                                   "description": "Unauthorized"
                                 },
                                 "403": {
                                   "description": "Forbidden"
                                 },
                                 "429": {
                                   "description": "Too many requests"
                                 }
                               },
                               "security": [
                                 {
                                   "Bearer": [ ]
                                 }
                               ]
                             }
                           },
                           "/api/events/outrights": {
                             "post": {
                               "tags": [
                                 "Feed"
                               ],
                               "summary": "Fetch Outright Events",
                               "requestBody": {
                                 "description": "Request",
                                 "content": {
                                   "application/json": {
                                     "schema": {
                                       "allOf": [
                                         {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.FeedFactory.QueryOutrightEventModel"
                                         }
                                       ]
                                     }
                                   },
                                   "text/json": {
                                     "schema": {
                                       "allOf": [
                                         {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.FeedFactory.QueryOutrightEventModel"
                                         }
                                       ]
                                     }
                                   },
                                   "application/*+json": {
                                     "schema": {
                                       "allOf": [
                                         {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.FeedFactory.QueryOutrightEventModel"
                                         }
                                       ]
                                     }
                                   }
                                 },
                                 "required": true
                               },
                               "responses": {
                                 "400": {
                                   "description": "Bad Request",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     }
                                   }
                                 },
                                 "500": {
                                   "description": "Internal Server Error"
                                 },
                                 "200": {
                                   "description": "OK",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.EventInfoViewModelV2"
                                         }
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.EventInfoViewModelV2"
                                         }
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.EventInfoViewModelV2"
                                         }
                                       }
                                     }
                                   }
                                 },
                                 "401": {
                                   "description": "Unauthorized"
                                 },
                                 "403": {
                                   "description": "Forbidden"
                                 },
                                 "429": {
                                   "description": "Too many requests"
                                 }
                               },
                               "security": [
                                 {
                                   "Bearer": [ ]
                                 }
                               ]
                             }
                           },
                           "/api/events/{eventId}": {
                             "get": {
                               "tags": [
                                 "Feed"
                               ],
                               "summary": "Get snapshot of the specific event.",
                               "operationId": "Get event snapshot",
                               "parameters": [
                                 {
                                   "name": "eventId",
                                   "in": "path",
                                   "description": "TradeArt event id.",
                                   "required": true,
                                   "schema": {
                                     "type": "integer",
                                     "format": "int64"
                                   }
                                 }
                               ],
                               "responses": {
                                 "200": {
                                   "description": "OK",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Models.FeedFactory.Delta.EventSnapshotDeltaViewModel"
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Models.FeedFactory.Delta.EventSnapshotDeltaViewModel"
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Models.FeedFactory.Delta.EventSnapshotDeltaViewModel"
                                       }
                                     }
                                   }
                                 },
                                 "410": {
                                   "description": "Gone",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "type": "string"
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "type": "string"
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "type": "string"
                                       }
                                     }
                                   }
                                 },
                                 "500": {
                                   "description": "Internal Server Error"
                                 },
                                 "401": {
                                   "description": "Unauthorized"
                                 },
                                 "403": {
                                   "description": "Forbidden"
                                 },
                                 "429": {
                                   "description": "Too many requests"
                                 }
                               },
                               "deprecated": true,
                               "security": [
                                 {
                                   "Bearer": [ ]
                                 }
                               ]
                             }
                           },
                           "/api/events/v2/{eventId}": {
                             "get": {
                               "tags": [
                                 "Feed"
                               ],
                               "summary": "Get snapshot of the specific event.",
                               "operationId": "Get event snapshot V2",
                               "parameters": [
                                 {
                                   "name": "eventId",
                                   "in": "path",
                                   "description": "TradeArt event id.",
                                   "required": true,
                                   "schema": {
                                     "type": "integer",
                                     "format": "int64"
                                   }
                                 }
                               ],
                               "responses": {
                                 "200": {
                                   "description": "OK",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Delta.EventSnapshotDeltaViewModelV2"
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Delta.EventSnapshotDeltaViewModelV2"
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Delta.EventSnapshotDeltaViewModelV2"
                                       }
                                     }
                                   }
                                 },
                                 "410": {
                                   "description": "Gone",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "type": "string"
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "type": "string"
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "type": "string"
                                       }
                                     }
                                   }
                                 },
                                 "500": {
                                   "description": "Internal Server Error"
                                 },
                                 "401": {
                                   "description": "Unauthorized"
                                 },
                                 "403": {
                                   "description": "Forbidden"
                                 },
                                 "429": {
                                   "description": "Too many requests"
                                 }
                               },
                               "security": [
                                 {
                                   "Bearer": [ ]
                                 }
                               ]
                             }
                           },
                           "/api/FixedExotics/price": {
                             "post": {
                               "tags": [
                                 "FixedExotics"
                               ],
                               "summary": "Get fixed exotics prices by specified racing selection, event and market IDs",
                               "operationId": "Get fixed exotic price by racing selection",
                               "requestBody": {
                                 "description": "View model containing all the parameters.",
                                 "content": {
                                   "application/json": {
                                     "schema": {
                                       "allOf": [
                                         {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.BetPlacement.GetFixedExoticPricesViewModel"
                                         }
                                       ],
                                       "description": "A model containing information needed for requesting fixed exotic market prices."
                                     }
                                   },
                                   "text/json": {
                                     "schema": {
                                       "allOf": [
                                         {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.BetPlacement.GetFixedExoticPricesViewModel"
                                         }
                                       ],
                                       "description": "A model containing information needed for requesting fixed exotic market prices."
                                     }
                                   },
                                   "application/*+json": {
                                     "schema": {
                                       "allOf": [
                                         {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.BetPlacement.GetFixedExoticPricesViewModel"
                                         }
                                       ],
                                       "description": "A model containing information needed for requesting fixed exotic market prices."
                                     }
                                   }
                                 }
                               },
                               "responses": {
                                 "200": {
                                   "description": "Getting prices successfully"
                                 },
                                 "400": {
                                   "description": "Request arguments are not valid.",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     }
                                   }
                                 },
                                 "404": {
                                   "description": "Event or market are not found.",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     }
                                   }
                                 },
                                 "500": {
                                   "description": "Unexpected error occurred."
                                 },
                                 "401": {
                                   "description": "Unauthorized"
                                 },
                                 "403": {
                                   "description": "Forbidden"
                                 },
                                 "429": {
                                   "description": "Too many requests"
                                 }
                               },
                               "security": [
                                 {
                                   "Bearer": [ ]
                                 }
                               ]
                             }
                           },
                           "/api/Metadata/sport/types/{sportId}": {
                             "get": {
                               "tags": [
                                 "Metadata"
                               ],
                               "summary": "Gets known metadata types for a specific sport.",
                               "parameters": [
                                 {
                                   "name": "sportId",
                                   "in": "path",
                                   "description": "TradeArt sport id.",
                                   "required": true,
                                   "schema": {
                                     "type": "integer",
                                     "format": "int64"
                                   }
                                 }
                               ],
                               "responses": {
                                 "200": {
                                   "description": "OK",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "type": "string"
                                         }
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "type": "string"
                                         }
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "type": "string"
                                         }
                                       }
                                     }
                                   }
                                 },
                                 "401": {
                                   "description": "Unauthorized"
                                 },
                                 "403": {
                                   "description": "Forbidden"
                                 },
                                 "429": {
                                   "description": "Too many requests"
                                 }
                               },
                               "security": [
                                 {
                                   "Bearer": [ ]
                                 }
                               ]
                             }
                           },
                           "/api/Metadata/event/types/{eventId}": {
                             "get": {
                               "tags": [
                                 "Metadata"
                               ],
                               "summary": "Gets all existing metadata types for a specific event.",
                               "parameters": [
                                 {
                                   "name": "eventId",
                                   "in": "path",
                                   "description": "TradeArt event id.",
                                   "required": true,
                                   "schema": {
                                     "type": "integer",
                                     "format": "int64"
                                   }
                                 }
                               ],
                               "responses": {
                                 "200": {
                                   "description": "OK",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "type": "string"
                                         }
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "type": "string"
                                         }
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "type": "string"
                                         }
                                       }
                                     }
                                   }
                                 },
                                 "401": {
                                   "description": "Unauthorized"
                                 },
                                 "403": {
                                   "description": "Forbidden"
                                 },
                                 "429": {
                                   "description": "Too many requests"
                                 }
                               },
                               "security": [
                                 {
                                   "Bearer": [ ]
                                 }
                               ]
                             }
                           },
                           "/api/Metadata/event/all/{eventId}": {
                             "get": {
                               "tags": [
                                 "Metadata"
                               ],
                               "summary": "Gets all latest metadata for a specific event.",
                               "parameters": [
                                 {
                                   "name": "eventId",
                                   "in": "path",
                                   "description": "TradeArt event id.",
                                   "required": true,
                                   "schema": {
                                     "type": "integer",
                                     "format": "int64"
                                   }
                                 }
                               ],
                               "responses": {
                                 "200": {
                                   "description": "OK",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Metadata.Model.EventMetadataViewModel"
                                         }
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Metadata.Model.EventMetadataViewModel"
                                         }
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Metadata.Model.EventMetadataViewModel"
                                         }
                                       }
                                     }
                                   }
                                 },
                                 "401": {
                                   "description": "Unauthorized"
                                 },
                                 "403": {
                                   "description": "Forbidden"
                                 },
                                 "429": {
                                   "description": "Too many requests"
                                 }
                               },
                               "security": [
                                 {
                                   "Bearer": [ ]
                                 }
                               ]
                             }
                           },
                           "/api/Metadata/event/{dataType}/{eventId}": {
                             "get": {
                               "tags": [
                                 "Metadata"
                               ],
                               "summary": "Gets metadata of specific type and version for a specific event.",
                               "parameters": [
                                 {
                                   "name": "eventId",
                                   "in": "path",
                                   "description": "TradeArt event id.",
                                   "required": true,
                                   "schema": {
                                     "type": "integer",
                                     "format": "int64"
                                   }
                                 },
                                 {
                                   "name": "dataType",
                                   "in": "path",
                                   "description": "Metadata type.",
                                   "required": true,
                                   "schema": {
                                     "type": "string"
                                   }
                                 },
                                 {
                                   "name": "version",
                                   "in": "query",
                                   "description": "Expected version of metadata. Will return latest if not specified.",
                                   "schema": {
                                     "type": "integer",
                                     "format": "int64"
                                   }
                                 }
                               ],
                               "responses": {
                                 "200": {
                                   "description": "OK",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Metadata.Model.EventMetadataViewModel"
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Metadata.Model.EventMetadataViewModel"
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Metadata.Model.EventMetadataViewModel"
                                       }
                                     }
                                   }
                                 },
                                 "404": {
                                   "description": "Not Found",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     }
                                   }
                                 },
                                 "401": {
                                   "description": "Unauthorized"
                                 },
                                 "403": {
                                   "description": "Forbidden"
                                 },
                                 "429": {
                                   "description": "Too many requests"
                                 }
                               },
                               "security": [
                                 {
                                   "Bearer": [ ]
                                 }
                               ]
                             }
                           },
                           "/api/schema/market-types": {
                             "get": {
                               "tags": [
                                 "Schema"
                               ],
                               "summary": "Gets all existing market types for a specific sport.",
                               "operationId": "Get market types",
                               "parameters": [
                                 {
                                   "name": "sportId",
                                   "in": "query",
                                   "description": "TradeArt sport id. Will return all market types if not specified.",
                                   "schema": {
                                     "type": "integer",
                                     "format": "int64"
                                   }
                                 },
                                 {
                                   "name": "includeFreeText",
                                   "in": "query",
                                   "description": "A flag indicating whether the response should include free text types or not.",
                                   "schema": {
                                     "type": "boolean",
                                     "default": true
                                   }
                                 }
                               ],
                               "responses": {
                                 "200": {
                                   "description": "OK",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Schema.MarketTypeViewModel"
                                         }
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Schema.MarketTypeViewModel"
                                         }
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Schema.MarketTypeViewModel"
                                         }
                                       }
                                     }
                                   }
                                 },
                                 "500": {
                                   "description": "Internal Server Error"
                                 },
                                 "401": {
                                   "description": "Unauthorized"
                                 },
                                 "403": {
                                   "description": "Forbidden"
                                 },
                                 "429": {
                                   "description": "Too many requests"
                                 }
                               },
                               "security": [
                                 {
                                   "Bearer": [ ]
                                 }
                               ]
                             }
                           },
                           "/api/schema/periods": {
                             "get": {
                               "tags": [
                                 "Schema"
                               ],
                               "summary": "Gets all existing periods for a specific sport.",
                               "operationId": "Get sport periods",
                               "parameters": [
                                 {
                                   "name": "sportId",
                                   "in": "query",
                                   "description": "TradeArt sport id. Will return all periods if not specified.",
                                   "schema": {
                                     "type": "integer",
                                     "format": "int64"
                                   }
                                 }
                               ],
                               "responses": {
                                 "200": {
                                   "description": "OK",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Schema.SportPeriodViewModel"
                                         }
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Schema.SportPeriodViewModel"
                                         }
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Schema.SportPeriodViewModel"
                                         }
                                       }
                                     }
                                   }
                                 },
                                 "401": {
                                   "description": "Unauthorized"
                                 },
                                 "403": {
                                   "description": "Forbidden"
                                 },
                                 "429": {
                                   "description": "Too many requests"
                                 }
                               },
                               "security": [
                                 {
                                   "Bearer": [ ]
                                 }
                               ]
                             }
                           },
                           "/api/schema/sports": {
                             "get": {
                               "tags": [
                                 "Schema"
                               ],
                               "summary": "Gets all existing sports.",
                               "operationId": "Get sports",
                               "responses": {
                                 "200": {
                                   "description": "OK",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Schema.SportViewModel"
                                         }
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Schema.SportViewModel"
                                         }
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Schema.SportViewModel"
                                         }
                                       }
                                     }
                                   }
                                 },
                                 "500": {
                                   "description": "Internal Server Error"
                                 },
                                 "401": {
                                   "description": "Unauthorized"
                                 },
                                 "403": {
                                   "description": "Forbidden"
                                 },
                                 "429": {
                                   "description": "Too many requests"
                                 }
                               },
                               "security": [
                                 {
                                   "Bearer": [ ]
                                 }
                               ]
                             }
                           },
                           "/api/translations/{lang}/sports": {
                             "get": {
                               "tags": [
                                 "Translations"
                               ],
                               "summary": "Gets translations to specific language for specified sports.",
                               "operationId": "Get sport translations",
                               "parameters": [
                                 {
                                   "name": "lang",
                                   "in": "path",
                                   "description": "Language code of translation.",
                                   "required": true,
                                   "schema": {
                                     "type": "string"
                                   }
                                 },
                                 {
                                   "name": "sports",
                                   "in": "query",
                                   "description": "Sports which will be translated.",
                                   "schema": {
                                     "type": "array",
                                     "items": {
                                       "type": "integer",
                                       "format": "int64"
                                     }
                                   }
                                 }
                               ],
                               "responses": {
                                 "200": {
                                   "description": "OK",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.TranslationViewModel"
                                         }
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.TranslationViewModel"
                                         }
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.TranslationViewModel"
                                         }
                                       }
                                     }
                                   }
                                 },
                                 "401": {
                                   "description": "Unauthorized"
                                 },
                                 "403": {
                                   "description": "Forbidden"
                                 },
                                 "429": {
                                   "description": "Too many requests"
                                 }
                               },
                               "security": [
                                 {
                                   "Bearer": [ ]
                                 }
                               ]
                             }
                           },
                           "/api/translations/{lang}/regions": {
                             "get": {
                               "tags": [
                                 "Translations"
                               ],
                               "summary": "Gets translations to specific language for specified regions.",
                               "operationId": "Get regions translations",
                               "parameters": [
                                 {
                                   "name": "lang",
                                   "in": "path",
                                   "description": "Language code for translation.",
                                   "required": true,
                                   "schema": {
                                     "type": "string"
                                   }
                                 },
                                 {
                                   "name": "regions",
                                   "in": "query",
                                   "description": "Regions which will be translated.",
                                   "schema": {
                                     "type": "array",
                                     "items": {
                                       "type": "integer",
                                       "format": "int64"
                                     }
                                   }
                                 }
                               ],
                               "responses": {
                                 "200": {
                                   "description": "OK",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.TranslationViewModel"
                                         }
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.TranslationViewModel"
                                         }
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.TranslationViewModel"
                                         }
                                       }
                                     }
                                   }
                                 },
                                 "401": {
                                   "description": "Unauthorized"
                                 },
                                 "403": {
                                   "description": "Forbidden"
                                 },
                                 "429": {
                                   "description": "Too many requests"
                                 }
                               },
                               "security": [
                                 {
                                   "Bearer": [ ]
                                 }
                               ]
                             }
                           },
                           "/api/translations/{lang}/leagues": {
                             "get": {
                               "tags": [
                                 "Translations"
                               ],
                               "summary": "Gets translations to specific language for specific leagues.",
                               "operationId": "Get leagues translations",
                               "parameters": [
                                 {
                                   "name": "lang",
                                   "in": "path",
                                   "description": "Language code for translation.",
                                   "required": true,
                                   "schema": {
                                     "type": "string"
                                   }
                                 },
                                 {
                                   "name": "leagues",
                                   "in": "query",
                                   "description": "Leagues which will be translated.",
                                   "schema": {
                                     "type": "array",
                                     "items": {
                                       "type": "integer",
                                       "format": "int64"
                                     }
                                   }
                                 }
                               ],
                               "responses": {
                                 "200": {
                                   "description": "OK",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.TranslationViewModel"
                                         }
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.TranslationViewModel"
                                         }
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.TranslationViewModel"
                                         }
                                       }
                                     }
                                   }
                                 },
                                 "401": {
                                   "description": "Unauthorized"
                                 },
                                 "403": {
                                   "description": "Forbidden"
                                 },
                                 "429": {
                                   "description": "Too many requests"
                                 }
                               },
                               "security": [
                                 {
                                   "Bearer": [ ]
                                 }
                               ]
                             }
                           },
                           "/api/translations/{lang}/competitors": {
                             "get": {
                               "tags": [
                                 "Translations"
                               ],
                               "summary": "Gets translations to specific language for specific competitors.",
                               "operationId": "Get competitors translations",
                               "parameters": [
                                 {
                                   "name": "lang",
                                   "in": "path",
                                   "description": "Language code for translation.",
                                   "required": true,
                                   "schema": {
                                     "type": "string"
                                   }
                                 },
                                 {
                                   "name": "competitors",
                                   "in": "query",
                                   "description": "Competitors which will be translated.",
                                   "schema": {
                                     "type": "array",
                                     "items": {
                                       "type": "integer",
                                       "format": "int64"
                                     }
                                   }
                                 }
                               ],
                               "responses": {
                                 "200": {
                                   "description": "OK",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.TranslationViewModel"
                                         }
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.TranslationViewModel"
                                         }
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.TranslationViewModel"
                                         }
                                       }
                                     }
                                   }
                                 },
                                 "401": {
                                   "description": "Unauthorized"
                                 },
                                 "403": {
                                   "description": "Forbidden"
                                 },
                                 "429": {
                                   "description": "Too many requests"
                                 }
                               },
                               "security": [
                                 {
                                   "Bearer": [ ]
                                 }
                               ]
                             }
                           },
                           "/api/translations/{lang}/{eventId}/markets": {
                             "get": {
                               "tags": [
                                 "Translations"
                               ],
                               "summary": "Gets translation to specific language for specific markets.",
                               "operationId": "Get markets translations",
                               "parameters": [
                                 {
                                   "name": "lang",
                                   "in": "path",
                                   "description": "Language code for translation.",
                                   "required": true,
                                   "schema": {
                                     "type": "string"
                                   }
                                 },
                                 {
                                   "name": "eventId",
                                   "in": "path",
                                   "description": "TradeArt event id.",
                                   "required": true,
                                   "schema": {
                                     "type": "integer",
                                     "format": "int64"
                                   }
                                 },
                                 {
                                   "name": "markets",
                                   "in": "query",
                                   "description": "Markets which will be translated.",
                                   "schema": {
                                     "type": "array",
                                     "items": {
                                       "type": "integer",
                                       "format": "int64"
                                     }
                                   }
                                 }
                               ],
                               "responses": {
                                 "200": {
                                   "description": "OK",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.MarketTranslationViewModel"
                                         }
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.MarketTranslationViewModel"
                                         }
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.MarketTranslationViewModel"
                                         }
                                       }
                                     }
                                   }
                                 },
                                 "500": {
                                   "description": "Internal Server Error"
                                 },
                                 "404": {
                                   "description": "Not Found",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     }
                                   }
                                 },
                                 "401": {
                                   "description": "Unauthorized"
                                 },
                                 "403": {
                                   "description": "Forbidden"
                                 },
                                 "429": {
                                   "description": "Too many requests"
                                 }
                               },
                               "security": [
                                 {
                                   "Bearer": [ ]
                                 }
                               ]
                             }
                           },
                           "/api/translations/all/sport/{sportId}": {
                             "get": {
                               "tags": [
                                 "Translations"
                               ],
                               "summary": "Gets all existing translations for a specific sport.",
                               "operationId": "Get all sport translations",
                               "parameters": [
                                 {
                                   "name": "sportId",
                                   "in": "path",
                                   "description": "TradeArt sport id.",
                                   "required": true,
                                   "schema": {
                                     "type": "integer",
                                     "format": "int64"
                                   }
                                 }
                               ],
                               "responses": {
                                 "200": {
                                   "description": "OK",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.EntityTranslationsViewModel"
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.EntityTranslationsViewModel"
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.EntityTranslationsViewModel"
                                       }
                                     }
                                   }
                                 },
                                 "401": {
                                   "description": "Unauthorized"
                                 },
                                 "403": {
                                   "description": "Forbidden"
                                 },
                                 "429": {
                                   "description": "Too many requests"
                                 }
                               },
                               "security": [
                                 {
                                   "Bearer": [ ]
                                 }
                               ]
                             }
                           },
                           "/api/translations/all/region/{regionId}": {
                             "get": {
                               "tags": [
                                 "Translations"
                               ],
                               "summary": "Gets all existing translations for a specific region.",
                               "operationId": "Get all region translations",
                               "parameters": [
                                 {
                                   "name": "regionId",
                                   "in": "path",
                                   "description": "TradeArt region id.",
                                   "required": true,
                                   "schema": {
                                     "type": "integer",
                                     "format": "int64"
                                   }
                                 }
                               ],
                               "responses": {
                                 "200": {
                                   "description": "OK",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.EntityTranslationsViewModel"
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.EntityTranslationsViewModel"
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.EntityTranslationsViewModel"
                                       }
                                     }
                                   }
                                 },
                                 "401": {
                                   "description": "Unauthorized"
                                 },
                                 "403": {
                                   "description": "Forbidden"
                                 },
                                 "429": {
                                   "description": "Too many requests"
                                 }
                               },
                               "security": [
                                 {
                                   "Bearer": [ ]
                                 }
                               ]
                             }
                           },
                           "/api/translations/all/league/{leagueId}": {
                             "get": {
                               "tags": [
                                 "Translations"
                               ],
                               "summary": "Gets all existing translations for a specific league.",
                               "operationId": "Get all league translations",
                               "parameters": [
                                 {
                                   "name": "leagueId",
                                   "in": "path",
                                   "description": "TradeArt league id.",
                                   "required": true,
                                   "schema": {
                                     "type": "integer",
                                     "format": "int64"
                                   }
                                 }
                               ],
                               "responses": {
                                 "200": {
                                   "description": "OK",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.EntityTranslationsViewModel"
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.EntityTranslationsViewModel"
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.EntityTranslationsViewModel"
                                       }
                                     }
                                   }
                                 },
                                 "401": {
                                   "description": "Unauthorized"
                                 },
                                 "403": {
                                   "description": "Forbidden"
                                 },
                                 "429": {
                                   "description": "Too many requests"
                                 }
                               },
                               "security": [
                                 {
                                   "Bearer": [ ]
                                 }
                               ]
                             }
                           },
                           "/api/translations/all/competitor/{competitorId}": {
                             "get": {
                               "tags": [
                                 "Translations"
                               ],
                               "summary": "Gets all existing translations for a specific competitor.",
                               "operationId": "Get all competitor translations",
                               "parameters": [
                                 {
                                   "name": "competitorId",
                                   "in": "path",
                                   "description": "TradeArt competitor id.",
                                   "required": true,
                                   "schema": {
                                     "type": "integer",
                                     "format": "int64"
                                   }
                                 }
                               ],
                               "responses": {
                                 "200": {
                                   "description": "OK",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.EntityTranslationsViewModel"
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.EntityTranslationsViewModel"
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.EntityTranslationsViewModel"
                                       }
                                     }
                                   }
                                 },
                                 "401": {
                                   "description": "Unauthorized"
                                 },
                                 "403": {
                                   "description": "Forbidden"
                                 },
                                 "429": {
                                   "description": "Too many requests"
                                 }
                               },
                               "security": [
                                 {
                                   "Bearer": [ ]
                                 }
                               ]
                             }
                           },
                           "/api/translations/all/event/{eventId}": {
                             "get": {
                               "tags": [
                                 "Translations"
                               ],
                               "summary": "Gets all existing translations for a specific event.",
                               "operationId": "Get all event translations",
                               "parameters": [
                                 {
                                   "name": "eventId",
                                   "in": "path",
                                   "description": "TradeArt event id.",
                                   "required": true,
                                   "schema": {
                                     "type": "integer",
                                     "format": "int64"
                                   }
                                 }
                               ],
                               "responses": {
                                 "200": {
                                   "description": "OK",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.EventTranslationsViewModel"
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.EventTranslationsViewModel"
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.EventTranslationsViewModel"
                                       }
                                     }
                                   }
                                 },
                                 "404": {
                                   "description": "Not Found",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     }
                                   }
                                 },
                                 "401": {
                                   "description": "Unauthorized"
                                 },
                                 "403": {
                                   "description": "Forbidden"
                                 },
                                 "429": {
                                   "description": "Too many requests"
                                 }
                               },
                               "security": [
                                 {
                                   "Bearer": [ ]
                                 }
                               ]
                             }
                           },
                           "/api/translations/all/{eventId}/markets": {
                             "get": {
                               "tags": [
                                 "Translations"
                               ],
                               "summary": "Gets all existing translations for specific markets.",
                               "operationId": "Get all market translations",
                               "parameters": [
                                 {
                                   "name": "eventId",
                                   "in": "path",
                                   "description": "TradeArt event id.",
                                   "required": true,
                                   "schema": {
                                     "type": "integer",
                                     "format": "int64"
                                   }
                                 },
                                 {
                                   "name": "markets",
                                   "in": "query",
                                   "description": "Markets which will be translated.",
                                   "schema": {
                                     "type": "array",
                                     "items": {
                                       "type": "integer",
                                       "format": "int64"
                                     }
                                   }
                                 }
                               ],
                               "responses": {
                                 "200": {
                                   "description": "OK",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.TranslatedMarketViewModel"
                                         }
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.TranslatedMarketViewModel"
                                         }
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "type": "array",
                                         "items": {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.TranslatedMarketViewModel"
                                         }
                                       }
                                     }
                                   }
                                 },
                                 "500": {
                                   "description": "Internal Server Error"
                                 },
                                 "404": {
                                   "description": "Not Found",
                                   "content": {
                                     "text/plain": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     },
                                     "application/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     },
                                     "text/json": {
                                       "schema": {
                                         "$ref": "#/components/schemas/Microsoft.AspNetCore.Mvc.ProblemDetails"
                                       }
                                     }
                                   }
                                 },
                                 "401": {
                                   "description": "Unauthorized"
                                 },
                                 "403": {
                                   "description": "Forbidden"
                                 },
                                 "429": {
                                   "description": "Too many requests"
                                 }
                               },
                               "security": [
                                 {
                                   "Bearer": [ ]
                                 }
                               ]
                             }
                           }
                         },
                         "components": {
                           "schemas": {
                             "AnimalRacingMetadata.AnimalRacingBasicEntity": {
                               "required": [
                                 "name"
                               ],
                               "type": "object",
                               "properties": {
                                 "id": {
                                   "type": "integer",
                                   "format": "int64",
                                   "deprecated": true
                                 },
                                 "name": {
                                   "type": "string",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "AnimalRacingMetadata.EventStatistics.AnimalRacingEventInfomatics": {
                               "type": "object",
                               "properties": {
                                 "eventRaceComment": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "eventCompetitorRaceComments": {
                                   "type": "object",
                                   "additionalProperties": {
                                     "$ref": "#/components/schemas/AnimalRacingMetadata.EventStatistics.AnimalRacingEventInfomaticsRaceComment"
                                   },
                                   "nullable": true
                                 },
                                 "eventRaceCommentSelections": {
                                   "type": "object",
                                   "additionalProperties": {
                                     "$ref": "#/components/schemas/AnimalRacingMetadata.EventStatistics.AnimalRacingInfomaticsEventRaceCommentSelection"
                                   },
                                   "nullable": true
                                 },
                                 "iRating": {
                                   "type": "object",
                                   "additionalProperties": {
                                     "$ref": "#/components/schemas/AnimalRacingMetadata.EventStatistics.AnimalRacingInfomaticsIRating"
                                   },
                                   "nullable": true
                                 },
                                 "speedMapEarlySpeed": {
                                   "type": "object",
                                   "additionalProperties": {
                                     "$ref": "#/components/schemas/AnimalRacingMetadata.EventStatistics.AnimalRacingEventInfomaticsSpeedMapEarlySpeed"
                                   },
                                   "nullable": true
                                 },
                                 "speedMapOnSettling": {
                                   "type": "object",
                                   "additionalProperties": {
                                     "$ref": "#/components/schemas/AnimalRacingMetadata.EventStatistics.AnimalRacingEventInfomaticsSpeedMapOnSettling"
                                   },
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "AnimalRacingMetadata.EventStatistics.AnimalRacingEventInfomaticsRaceComment": {
                               "type": "object",
                               "properties": {
                                 "comment": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "programNumber": {
                                   "type": "string",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "AnimalRacingMetadata.EventStatistics.AnimalRacingEventInfomaticsSpeedMapEarlySpeed": {
                               "type": "object",
                               "properties": {
                                 "position": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "programNumber": {
                                   "type": "string",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "AnimalRacingMetadata.EventStatistics.AnimalRacingEventInfomaticsSpeedMapOnSettling": {
                               "type": "object",
                               "properties": {
                                 "positions": {
                                   "type": "array",
                                   "items": {
                                     "$ref": "#/components/schemas/AnimalRacingMetadata.EventStatistics.AnimalRacingEventInfomaticsSpeedMapOnSettlingsPosition"
                                   },
                                   "nullable": true
                                 },
                                 "programNumber": {
                                   "type": "string",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "AnimalRacingMetadata.EventStatistics.AnimalRacingEventInfomaticsSpeedMapOnSettlingsPosition": {
                               "type": "object",
                               "properties": {
                                 "percentage": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "position": {
                                   "type": "string",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "AnimalRacingMetadata.EventStatistics.AnimalRacingEventStatistics": {
                               "type": "object",
                               "properties": {
                                 "runnerStatistics": {
                                   "type": "object",
                                   "additionalProperties": {
                                     "$ref": "#/components/schemas/AnimalRacingMetadata.EventStatistics.AnimalRacingEventStatisticsRunnerStats"
                                   },
                                   "nullable": true
                                 },
                                 "runnerLastRuns": {
                                   "type": "object",
                                   "additionalProperties": {
                                     "$ref": "#/components/schemas/AnimalRacingMetadata.EventStatistics.AnimalRacingEventStatisticsLastRun"
                                   },
                                   "nullable": true
                                 },
                                 "infomatics": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/AnimalRacingMetadata.EventStatistics.AnimalRacingEventInfomatics"
                                     }
                                   ],
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "AnimalRacingMetadata.EventStatistics.AnimalRacingEventStatisticsLastRun": {
                               "type": "object",
                               "properties": {
                                 "all": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "last6": {
                                   "type": "string",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "AnimalRacingMetadata.EventStatistics.AnimalRacingEventStatisticsRunnerStats": {
                               "type": "object",
                               "properties": {
                                 "all": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/AnimalRacingMetadata.EventStatistics.AnimalRacingEventStatisticsSingleRunnerStat"
                                     }
                                   ],
                                   "nullable": true
                                 },
                                 "distance": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/AnimalRacingMetadata.EventStatistics.AnimalRacingEventStatisticsSingleRunnerStat"
                                     }
                                   ],
                                   "nullable": true
                                 },
                                 "venue": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/AnimalRacingMetadata.EventStatistics.AnimalRacingEventStatisticsSingleRunnerStat"
                                     }
                                   ],
                                   "nullable": true
                                 },
                                 "venueDistance": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/AnimalRacingMetadata.EventStatistics.AnimalRacingEventStatisticsSingleRunnerStat"
                                     }
                                   ],
                                   "nullable": true
                                 },
                                 "lastYear": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/AnimalRacingMetadata.EventStatistics.AnimalRacingEventStatisticsSingleRunnerStat"
                                     }
                                   ],
                                   "nullable": true
                                 },
                                 "firstJump": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/AnimalRacingMetadata.EventStatistics.AnimalRacingEventStatisticsSingleRunnerStat"
                                     }
                                   ],
                                   "nullable": true
                                 },
                                 "secondJump": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/AnimalRacingMetadata.EventStatistics.AnimalRacingEventStatisticsSingleRunnerStat"
                                     }
                                   ],
                                   "nullable": true
                                 },
                                 "thirdJump": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/AnimalRacingMetadata.EventStatistics.AnimalRacingEventStatisticsSingleRunnerStat"
                                     }
                                   ],
                                   "nullable": true
                                 },
                                 "going": {
                                   "type": "object",
                                   "additionalProperties": {
                                     "$ref": "#/components/schemas/AnimalRacingMetadata.EventStatistics.AnimalRacingEventStatisticsSingleRunnerStat"
                                   },
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "AnimalRacingMetadata.EventStatistics.AnimalRacingEventStatisticsSingleRunnerStat": {
                               "type": "object",
                               "properties": {
                                 "starts": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "firsts": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "seconds": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "thirds": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "AnimalRacingMetadata.EventStatistics.AnimalRacingInfomaticsEventRaceCommentSelection": {
                               "required": [
                                 "competitorId"
                               ],
                               "type": "object",
                               "properties": {
                                 "competitorId": {
                                   "type": "integer",
                                   "format": "int64"
                                 },
                                 "name": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "programNumber": {
                                   "type": "string",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "AnimalRacingMetadata.EventStatistics.AnimalRacingInfomaticsIRating": {
                               "type": "object",
                               "properties": {
                                 "programNumber": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "rating": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "AnimalRacingMetadata.RaceInfo.AnimalRacingEventResultDividend": {
                               "type": "object",
                               "properties": {
                                 "marketId": {
                                   "type": "integer",
                                   "format": "int64"
                                 },
                                 "result": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "dividend": {
                                   "type": "number",
                                   "format": "double"
                                 }
                               },
                               "additionalProperties": false
                             },
                             "AnimalRacingMetadata.RaceInfo.AnimalRacingEventResultFinisher": {
                               "type": "object",
                               "properties": {
                                 "bettingInterestNumber": {
                                   "type": "integer",
                                   "format": "int32"
                                 },
                                 "eventCompetitorId": {
                                   "type": "integer",
                                   "format": "int64"
                                 },
                                 "finishPosition": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "programNumber": {
                                   "type": "integer",
                                   "format": "int32"
                                 }
                               },
                               "additionalProperties": false
                             },
                             "AnimalRacingMetadata.RaceInfo.AnimalRacingEventResults": {
                               "type": "object",
                               "properties": {
                                 "finishers": {
                                   "type": "object",
                                   "additionalProperties": {
                                     "type": "array",
                                     "items": {
                                       "$ref": "#/components/schemas/AnimalRacingMetadata.RaceInfo.AnimalRacingEventResultFinisher"
                                     },
                                     "nullable": true
                                   },
                                   "nullable": true
                                 },
                                 "dividends": {
                                   "type": "array",
                                   "items": {
                                     "$ref": "#/components/schemas/AnimalRacingMetadata.RaceInfo.AnimalRacingEventResultDividend"
                                   },
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "AnimalRacingMetadata.RaceInfo.AnimalRacingRaceInfo": {
                               "required": [
                                 "eventNumber"
                               ],
                               "type": "object",
                               "properties": {
                                 "distance": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "distanceUnit": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "prizeMoney": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "prizeMoneyCurrency": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "weatherDescription": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "raceClass": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "railPosition": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "trackType": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "trackLayout": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "trackCondition": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "courseDirection": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "eventNumber": {
                                   "type": "integer",
                                   "format": "int64"
                                 },
                                 "meetingDate": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "trackName": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "trackCountryCode": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "trackCountryCode2": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "deductions": {
                                   "type": "array",
                                   "items": {
                                     "$ref": "#/components/schemas/AnimalRacingMetadata.RaceInfo.AnimalRacingRaceInfoDeduction"
                                   },
                                   "nullable": true
                                 },
                                 "results": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/AnimalRacingMetadata.RaceInfo.AnimalRacingEventResults"
                                     }
                                   ],
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "AnimalRacingMetadata.RaceInfo.AnimalRacingRaceInfoDeduction": {
                               "type": "object",
                               "properties": {
                                 "scratchedRunnerId": {
                                   "type": "integer",
                                   "format": "int64",
                                   "nullable": true
                                 },
                                 "runnerScratchedAt": {
                                   "type": "integer",
                                   "format": "int64"
                                 },
                                 "deductionPercentage": {
                                   "type": "number",
                                   "format": "double"
                                 },
                                 "marketId": {
                                   "type": "integer",
                                   "format": "int64"
                                 }
                               },
                               "additionalProperties": false
                             },
                             "AnimalRacingMetadata.RaceRunnerForms.AnimalRacingRaceFinisher": {
                               "type": "object",
                               "properties": {
                                 "id": {
                                   "type": "integer",
                                   "format": "int64",
                                   "nullable": true,
                                   "deprecated": true
                                 },
                                 "name": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "finishTime": {
                                   "type": "number",
                                   "format": "float",
                                   "nullable": true
                                 },
                                 "margin": {
                                   "type": "number",
                                   "format": "double",
                                   "nullable": true
                                 },
                                 "handicapWeight": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "handicapWeightUnit": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "barrierNumber": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "trainerName": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "jockeyName": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "spPrice": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "position": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "AnimalRacingMetadata.RaceRunnerForms.AnimalRacingRaceRunnerForms": {
                               "required": [
                                 "runnerForms"
                               ],
                               "type": "object",
                               "properties": {
                                 "runnerForms": {
                                   "type": "object",
                                   "additionalProperties": {
                                     "type": "array",
                                     "items": {
                                       "$ref": "#/components/schemas/AnimalRacingMetadata.RaceRunnerForms.AnimalRacingRunnerRaceForm"
                                     }
                                   },
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "AnimalRacingMetadata.RaceRunnerForms.AnimalRacingRunnerRaceForm": {
                               "required": [
                                 "competitorId"
                               ],
                               "type": "object",
                               "properties": {
                                 "meetingId": {
                                   "type": "integer",
                                   "format": "int64",
                                   "deprecated": true
                                 },
                                 "eventId": {
                                   "type": "integer",
                                   "format": "int64",
                                   "deprecated": true
                                 },
                                 "eventCompetitorId": {
                                   "type": "integer",
                                   "format": "int64",
                                   "deprecated": true
                                 },
                                 "trainerId": {
                                   "type": "integer",
                                   "format": "int64",
                                   "deprecated": true
                                 },
                                 "jockeyId": {
                                   "type": "integer",
                                   "format": "int64",
                                   "deprecated": true
                                 },
                                 "eventName": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "competitorId": {
                                   "type": "integer",
                                   "format": "int64"
                                 },
                                 "venueName": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "meetingDate": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "countryName": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "eventNumber": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "eventPostTime": {
                                   "type": "integer",
                                   "format": "int64",
                                   "nullable": true
                                 },
                                 "competitorName": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "trainerName": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "jockeyName": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "programNumber": {
                                   "type": "integer",
                                   "format": "int64",
                                   "nullable": true
                                 },
                                 "barrierNumber": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "horseWeight": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "horseWeightUnit": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "handicapWeight": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "handicapWeightUnit": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "finishTime": {
                                   "type": "number",
                                   "format": "float",
                                   "nullable": true
                                 },
                                 "finishPosition": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "failedToFinishReason": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "fieldSize": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "rating": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "distance": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "distanceUnit": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "trackType": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "trackCondition": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "railPosition": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "raceClass": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "gear": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "positionsInRun": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "sectionalTimes": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "winMargin": {
                                   "type": "number",
                                   "format": "double",
                                   "nullable": true
                                 },
                                 "spPrice": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "spPriceRs": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "finishers": {
                                   "type": "array",
                                   "items": {
                                     "$ref": "#/components/schemas/AnimalRacingMetadata.RaceRunnerForms.AnimalRacingRaceFinisher"
                                   },
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "AnimalRacingMetadata.RaceRunners.AnimalRacingCompetitor": {
                               "required": [
                                 "id"
                               ],
                               "type": "object",
                               "properties": {
                                 "id": {
                                   "type": "integer",
                                   "format": "int64"
                                 },
                                 "name": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "birthDate": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "birthCountry": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "colour": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "sex": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "jcr": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "importType": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "formerName": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "sire": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/AnimalRacingMetadata.AnimalRacingBasicEntity"
                                     }
                                   ],
                                   "nullable": true
                                 },
                                 "dam": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/AnimalRacingMetadata.AnimalRacingBasicEntity"
                                     }
                                   ],
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "AnimalRacingMetadata.RaceRunners.AnimalRacingRaceRunner": {
                               "required": [
                                 "barrierNumber",
                                 "competitor",
                                 "id"
                               ],
                               "type": "object",
                               "properties": {
                                 "id": {
                                   "type": "integer",
                                   "format": "int64"
                                 },
                                 "programNumber": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "bettingInterestNumber": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "barrierNumber": {
                                   "type": "integer",
                                   "format": "int32"
                                 },
                                 "scratched": {
                                   "type": "boolean"
                                 },
                                 "scratchedAt": {
                                   "type": "string",
                                   "format": "date-time",
                                   "nullable": true
                                 },
                                 "jockey": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/AnimalRacingMetadata.AnimalRacingBasicEntity"
                                     }
                                   ],
                                   "nullable": true
                                 },
                                 "trainer": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/AnimalRacingMetadata.AnimalRacingBasicEntity"
                                     }
                                   ],
                                   "nullable": true
                                 },
                                 "silkUri": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "finishPosition": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "gear": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "handicapWeight": {
                                   "type": "number",
                                   "format": "double",
                                   "nullable": true
                                 },
                                 "handicapWeightUnit": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "weight": {
                                   "type": "number",
                                   "format": "double",
                                   "nullable": true
                                 },
                                 "weightUnit": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "competitor": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/AnimalRacingMetadata.RaceRunners.AnimalRacingCompetitor"
                                     }
                                   ],
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "AnimalRacingMetadata.RaceRunners.AnimalRacingRaceRunners": {
                               "required": [
                                 "runners"
                               ],
                               "type": "object",
                               "properties": {
                                 "runners": {
                                   "type": "array",
                                   "items": {
                                     "$ref": "#/components/schemas/AnimalRacingMetadata.RaceRunners.AnimalRacingRaceRunner"
                                   },
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "CricketMetadata.CricketScoreMetadata.Batsman": {
                               "type": "object",
                               "properties": {
                                 "batsmanName": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "active": {
                                   "type": "boolean"
                                 },
                                 "onStrike": {
                                   "type": "boolean"
                                 },
                                 "fours": {
                                   "type": "integer",
                                   "format": "int32"
                                 },
                                 "sixes": {
                                   "type": "integer",
                                   "format": "int32"
                                 },
                                 "runs": {
                                   "type": "integer",
                                   "format": "int32"
                                 },
                                 "balls": {
                                   "type": "integer",
                                   "format": "int32"
                                 },
                                 "playerId": {
                                   "type": "string",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "CricketMetadata.CricketScoreMetadata.BattingTeam": {
                               "type": "object",
                               "properties": {
                                 "teamName": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "teamRuns": {
                                   "type": "integer",
                                   "format": "int32"
                                 },
                                 "teamWickets": {
                                   "type": "integer",
                                   "format": "int32"
                                 },
                                 "teamOvers": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "fours": {
                                   "type": "integer",
                                   "format": "int32"
                                 },
                                 "sixes": {
                                   "type": "integer",
                                   "format": "int32"
                                 },
                                 "extras": {
                                   "type": "integer",
                                   "format": "int32"
                                 },
                                 "competitorId": {
                                   "type": "string",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "CricketMetadata.CricketScoreMetadata.CurrentBowler": {
                               "type": "object",
                               "properties": {
                                 "bowlerName": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "playerId": {
                                   "type": "string",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "CricketMetadata.CricketScoreMetadata.Over": {
                               "type": "object",
                               "properties": {
                                 "overNumber": {
                                   "type": "integer",
                                   "format": "int32"
                                 },
                                 "runs": {
                                   "type": "integer",
                                   "format": "int32"
                                 },
                                 "isCurrentOver": {
                                   "type": "boolean"
                                 },
                                 "balls": {
                                   "type": "array",
                                   "items": {
                                     "type": "string"
                                   },
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "CricketMetadata.CricketScoreMetadata.PreviousInning": {
                               "type": "object",
                               "properties": {
                                 "inningsNumber": {
                                   "type": "integer",
                                   "format": "int32"
                                 },
                                 "runs": {
                                   "type": "integer",
                                   "format": "int32"
                                 },
                                 "wickets": {
                                   "type": "integer",
                                   "format": "int32"
                                 },
                                 "overs": {
                                   "type": "number",
                                   "format": "double"
                                 },
                                 "oversAvailable": {
                                   "type": "number",
                                   "format": "double"
                                 },
                                 "teamName": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "conclusion": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "summary": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "competitorId": {
                                   "type": "string",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "CricketMetadata.CricketScoreMetadata.Score": {
                               "type": "object",
                               "properties": {
                                 "seriesName": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "matchTitle": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "matchCommentary": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "battingTeam": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/CricketMetadata.CricketScoreMetadata.BattingTeam"
                                     }
                                   ],
                                   "nullable": true
                                 },
                                 "currentBowler": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/CricketMetadata.CricketScoreMetadata.CurrentBowler"
                                     }
                                   ],
                                   "nullable": true
                                 },
                                 "batsmen": {
                                   "type": "array",
                                   "items": {
                                     "$ref": "#/components/schemas/CricketMetadata.CricketScoreMetadata.Batsman"
                                   },
                                   "nullable": true
                                 },
                                 "previousInnings": {
                                   "type": "array",
                                   "items": {
                                     "$ref": "#/components/schemas/CricketMetadata.CricketScoreMetadata.PreviousInning"
                                   },
                                   "nullable": true
                                 },
                                 "overs": {
                                   "type": "array",
                                   "items": {
                                     "$ref": "#/components/schemas/CricketMetadata.CricketScoreMetadata.Over"
                                   },
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "Marlin.SportsbetApi.Host.Authentication.LoginModel": {
                               "required": [
                                 "clientId",
                                 "password"
                               ],
                               "type": "object",
                               "properties": {
                                 "clientId": {
                                   "minLength": 1,
                                   "type": "string"
                                 },
                                 "password": {
                                   "minLength": 1,
                                   "type": "string"
                                 }
                               },
                               "additionalProperties": false
                             },
                             "Marlin.SportsbetApi.Host.FeedFactory.QueryOutrightEventModel": {
                               "type": "object",
                               "properties": {
                                 "sportIds": {
                                   "type": "array",
                                   "items": {
                                     "type": "integer",
                                     "format": "int64"
                                   },
                                   "description": "If not null filters by SportIds",
                                   "nullable": true
                                 },
                                 "locationIds": {
                                   "type": "array",
                                   "items": {
                                     "type": "integer",
                                     "format": "int64"
                                   },
                                   "description": "If not null filters by LocationIds",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "Marlin.SportsbetApi.Host.FeedFactory.QueryTradingEventModel": {
                               "type": "object",
                               "properties": {
                                 "startDate": {
                                   "type": "string",
                                   "description": "Starting date of event. If null returns Events with StartTime=null or from tomorrow",
                                   "nullable": true
                                 },
                                 "sportIds": {
                                   "type": "array",
                                   "items": {
                                     "type": "integer",
                                     "format": "int64"
                                   },
                                   "description": "If not null filters by SportIds",
                                   "nullable": true
                                 },
                                 "locationIds": {
                                   "type": "array",
                                   "items": {
                                     "type": "integer",
                                     "format": "int64"
                                   },
                                   "description": "If not null filters by LocationIds",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "Marlin.SportsbetApi.Host.Models.BetPlacement.BetItemViewModel": {
                               "type": "object",
                               "properties": {
                                 "eventId": {
                                   "type": "integer",
                                   "description": "Event id on which the bet is placed.",
                                   "format": "int64"
                                 },
                                 "marketId": {
                                   "type": "integer",
                                   "description": "Market id on which the bet is placed.",
                                   "format": "int64"
                                 },
                                 "selectionId": {
                                   "type": "integer",
                                   "description": "Selection id on which the bet is placed. Should be `-1` for exotics bet.",
                                   "format": "int64"
                                 },
                                 "racingSelection": {
                                   "type": "string",
                                   "description": "Racing exotics selection.",
                                   "nullable": true
                                 },
                                 "price": {
                                   "type": "number",
                                   "description": "Price on which the bet is placed. Should be `null` for tote exotics and SP bet.",
                                   "format": "double",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false,
                               "description": "Represents item inside the bet."
                             },
                             "Marlin.SportsbetApi.Host.Models.BetPlacement.BetRequestViewModel": {
                               "type": "object",
                               "properties": {
                                 "id": {
                                   "type": "string",
                                   "description": "Unique request id (in the client's system);",
                                   "nullable": true
                                 },
                                 "player": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.BetPlacement.PlayerViewModel"
                                     }
                                   ],
                                   "description": "Identification of the end user. Contains information about Ip, language and device from which bet was placed.",
                                   "nullable": true
                                 },
                                 "amount": {
                                   "type": "number",
                                   "description": "Bet amount in EUR currency",
                                   "format": "double"
                                 },
                                 "priceChange": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.BetPlacement.PriceChange"
                                     }
                                   ],
                                   "description": "Price change auto acceptance. (None - 0, Any - 1, Higher - 2)\n\n0 = None (No price changes are accepted.)\n\n1 = Any (Any price change is accepted.)\n\n2 = Higher (Only higher price will be accepted.)",
                                   "x-enumNames": [
                                     "None",
                                     "Any",
                                     "Higher"
                                   ],
                                   "x-enumDescriptions": [
                                     "No price changes are accepted.",
                                     "Any price change is accepted.",
                                     "Only higher price will be accepted."
                                   ]
                                 },
                                 "items": {
                                   "type": "array",
                                   "items": {
                                     "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.BetPlacement.BetItemViewModel"
                                   },
                                   "description": "List of bet items",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false,
                               "description": "Represents bet request."
                             },
                             "Marlin.SportsbetApi.Host.Models.BetPlacement.BetResultViewModel": {
                               "type": "object",
                               "properties": {
                                 "requestId": {
                                   "type": "string",
                                   "description": "External request id which was sent by client",
                                   "nullable": true
                                 },
                                 "resultType": {
                                   "type": "integer",
                                   "description": "Acceptance status. (Pending - 1 , Accepted - 2, Rejected -3)",
                                   "format": "int32"
                                 },
                                 "rejectInfo": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.BetPlacement.RejectInfoViewModel"
                                     }
                                   ],
                                   "description": "Information about rejection cause and rejected item",
                                   "nullable": true
                                 },
                                 "createdAt": {
                                   "type": "string",
                                   "description": "Date and time of request creation in UTC",
                                   "format": "date-time"
                                 },
                                 "acceptedBet": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.BetPlacement.BetViewModel"
                                     }
                                   ],
                                   "description": "Accepted bet",
                                   "nullable": true
                                 },
                                 "cancellationStatus": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.BetPlacement.CancellationStatusViewModel"
                                     }
                                   ],
                                   "description": "Detailed info about cancellation progress",
                                   "nullable": true
                                 },
                                 "betProcessor": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/TradeArt.BetPlacement.GrainContracts.Enums.BetProcessorType"
                                     }
                                   ],
                                   "description": "\n\n1 = Internal\n\n2 = Betradar",
                                   "x-enumNames": [
                                     "Internal",
                                     "Betradar"
                                   ],
                                   "x-enumDescriptions": [
                                     "",
                                     ""
                                   ]
                                 }
                               },
                               "additionalProperties": false
                             },
                             "Marlin.SportsbetApi.Host.Models.BetPlacement.BetViewModel": {
                               "type": "object",
                               "properties": {
                                 "id": {
                                   "type": "string",
                                   "description": "Auto generated identifier of accepted bet",
                                   "nullable": true
                                 },
                                 "player": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.BetPlacement.PlayerViewModel"
                                     }
                                   ],
                                   "description": "Identification of the end user. Contains information about Ip, language and device from which bet was placed.",
                                   "nullable": true
                                 },
                                 "amount": {
                                   "type": "number",
                                   "description": "Bet amount in EUR currency",
                                   "format": "double"
                                 },
                                 "kind": {
                                   "type": "integer",
                                   "description": "Bet kind. (Single - 0, Multiple - 1)",
                                   "format": "int32"
                                 },
                                 "priceChange": {
                                   "type": "integer",
                                   "description": "Price change auto acceptance. (None - 0, Any - 1, Higher - 2)",
                                   "format": "int32"
                                 },
                                 "items": {
                                   "type": "array",
                                   "items": {
                                     "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.BetPlacement.BetItemViewModel"
                                   },
                                   "description": "List of bet items",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "Marlin.SportsbetApi.Host.Models.BetPlacement.CancelBetViewModel": {
                               "type": "object",
                               "properties": {
                                 "reason": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.BetPlacement.CancellationReasonType"
                                     }
                                   ],
                                   "description": "Cancellation reason. TimeOutTriggered = 2, BookmakerBackofficeTriggered = 3\n\n2 = TimeOutTriggered\n\n3 = BookmakerBackofficeTriggered",
                                   "x-enumNames": [
                                     "TimeOutTriggered",
                                     "BookmakerBackofficeTriggered"
                                   ],
                                   "x-enumDescriptions": [
                                     "",
                                     ""
                                   ]
                                 }
                               },
                               "additionalProperties": false
                             },
                             "Marlin.SportsbetApi.Host.Models.BetPlacement.CancellationReasonType": {
                               "enum": [
                                 2,
                                 3
                               ],
                               "type": "integer",
                               "description": "\n\n2 = TimeOutTriggered\n\n3 = BookmakerBackofficeTriggered",
                               "format": "int32",
                               "x-enumNames": [
                                 "TimeOutTriggered",
                                 "BookmakerBackofficeTriggered"
                               ],
                               "x-enumDescriptions": [
                                 "",
                                 ""
                               ]
                             },
                             "Marlin.SportsbetApi.Host.Models.BetPlacement.CancellationStatusViewModel": {
                               "type": "object",
                               "properties": {
                                 "status": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/TradeArt.BetPlacement.GrainContracts.Enums.CancellationStatusType"
                                     }
                                   ],
                                   "description": "Cancellation status\n\n1 = Accepted\n\n2 = Rejected",
                                   "x-enumNames": [
                                     "Accepted",
                                     "Rejected"
                                   ],
                                   "x-enumDescriptions": [
                                     "",
                                     ""
                                   ]
                                 },
                                 "message": {
                                   "type": "string",
                                   "description": "Cancellation rejection reason description",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "Marlin.SportsbetApi.Host.Models.BetPlacement.GetFixedExoticPricesViewModel": {
                               "type": "object",
                               "properties": {
                                 "eventId": {
                                   "type": "integer",
                                   "description": "Event ID.",
                                   "format": "int64"
                                 },
                                 "marketId": {
                                   "type": "integer",
                                   "description": "Market ID.",
                                   "format": "int64"
                                 },
                                 "racingSelection": {
                                   "type": "string",
                                   "description": "Racing selection.",
                                   "nullable": true
                                 },
                                 "correlationId": {
                                   "type": "string",
                                   "description": "Correlation ID (optional).",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false,
                               "description": "A model containing information needed for requesting fixed exotic market prices."
                             },
                             "Marlin.SportsbetApi.Host.Models.BetPlacement.MaxBetResultViewModel": {
                               "type": "object",
                               "properties": {
                                 "rejectReason": {
                                   "type": "integer",
                                   "description": "Information about rejection cause. (None - 0, UnknownError - 1, PriceChanged - 2, LimitExceeded - 3 , SelectionNotFound - 4 etc.)",
                                   "format": "int32"
                                 },
                                 "amount": {
                                   "type": "number",
                                   "description": "Max bet amount in EUR currency",
                                   "format": "double"
                                 }
                               },
                               "additionalProperties": false
                             },
                             "Marlin.SportsbetApi.Host.Models.BetPlacement.PlayerViewModel": {
                               "type": "object",
                               "properties": {
                                 "deviceId": {
                                   "type": "string",
                                   "description": "End user's device id",
                                   "nullable": true
                                 },
                                 "languageId": {
                                   "type": "string",
                                   "description": "ISO 639-1 language code",
                                   "nullable": true
                                 },
                                 "ip": {
                                   "type": "string",
                                   "description": "End user's IP",
                                   "nullable": true
                                 },
                                 "segmentId": {
                                   "type": "integer",
                                   "description": "End user's limit Id",
                                   "format": "int32"
                                 },
                                 "channel": {
                                   "type": "string",
                                   "description": "End user's channel",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false,
                               "description": "Represents player data."
                             },
                             "Marlin.SportsbetApi.Host.Models.BetPlacement.PriceChange": {
                               "enum": [
                                 0,
                                 1,
                                 2
                               ],
                               "type": "integer",
                               "description": "Types of price change acceptance.\n\n0 = None (No price changes are accepted.)\n\n1 = Any (Any price change is accepted.)\n\n2 = Higher (Only higher price will be accepted.)",
                               "format": "int32",
                               "x-enumNames": [
                                 "None",
                                 "Any",
                                 "Higher"
                               ],
                               "x-enumDescriptions": [
                                 "No price changes are accepted.",
                                 "Any price change is accepted.",
                                 "Only higher price will be accepted."
                               ]
                             },
                             "Marlin.SportsbetApi.Host.Models.BetPlacement.RejectInfoViewModel": {
                               "type": "object",
                               "properties": {
                                 "reason": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/TradeArt.BetPlacement.GrainContracts.Enums.RejectReason"
                                     }
                                   ],
                                   "description": "Information about rejection cause\n\n0 = None (Checks passed)\n\n1 = PriceChanged (Price Changed)\n\n2 = PriceChangedLow (Price Changed Low)\n\n3 = PriceChangedHigh (Price Changed High)\n\n4 = MissingMandatoryFields (Missing Mandatory Fields)\n\n5 = InvalidDataFormat (Invalid Data Format)\n\n6 = EventsOrMarketsCannotBeCombined (Events Or Markets Cannot Be Combined)\n\n7 = SelectionsFromSameMarket (Selections From Same Market)\n\n8 = MultipleSelectionsInEvent (Multiple Selections In Event)\n\n9 = DuplicateTeamIds (Duplicate Team Ids)\n\n10 = EventNotFound (Event Not Found)\n\n11 = EventNotActive (Event Not Active)\n\n12 = MarketNotFound (Market Not Found)\n\n13 = MarketNotActive (Market Not Active)\n\n14 = SelectionNotFound (Selection Not Found)\n\n15 = SelectionNotActive (Selection Not Active)\n\n16 = AdminConsoleError (Admin Console Error)\n\n17 = EventLimitExceeded (Event Limit Exceeded)\n\n18 = MarketLimitExceeded (Market Limit Exceeded)\n\n19 = BetLimitExceeded (Bet Limit Exceeded)\n\n20 = PayoutLimitExceeded (Payout Limit Exceeded)\n\n21 = EventVarLimitExceeded (Event CCF Limit Exceeded)\n\n22 = MarketVarLimitExceeded (Market CCF Limit Exceeded)\n\n23 = BetVarLimitExceeded (Bet CCF Limit Exceeded)\n\n24 = PayoutVarLimitExceeded (Payout CCF Limit Exceeded)\n\n25 = TooHighOdds (Total price was too large)\n\n26 = SportRestricted (Sport restricted for placing bets)\n\n27 = EventMarkedStale (Event has not received any updates from source for too long and bets cannot be accepted for it)\n\n28 = ResponseTimeoutExceeded (Bet placement response timed out)\n\n29 = ExoticSelectionInvalidFormat (Exotics selection has invalid format)\n\n39 = ExoticTypeNotMatchWithMarket (Exotics selection type does not match with market type)\n\n40 = ExoticsMultibetNotSupported (Exotics multibet is not supported)\n\n50 = SelectionLimitExceeded (Prematch selection Limit Exceeded)\n\n51 = SelectionVarLimitExceeded (Prematch selection CCF Limit Exceeded)\n\n52 = MarketIsNotFixedExotics (Requested market is not Fixed Exotics market)\n\n53 = FixedExoticsPriceNotFoundBySelection (The fixed exotics price was not found by selection)\n\n54 = FixedExoticsSelectionContainsMultipleCombinations (Multiple combinations are not supported for fixed exotics)\n\n55 = PlayerBetLimitExceeded (Liability is over player bet liability limit)\n\n56 = StartingPriceMarketLive (Starting price market went live)\n\n57 = StartingPriceMultibetNotSupported (Starting price multibet is not supported)\n\n58 = StartingPriceMarketMatch (Starting price market bets should not contain price)\n\n59 = NullBetPriceIsNotSupportedForThisMarket (Null bet price is not supported for this market)\n\n60 = CombinationContainsScratchedCompetitorNumber (Racing selection contains scratched competitor)\n\n61 = BetBuilderNotSupported (Bet Builder is not supported)\n\n100 = UnexpectedError (Something went wrong)",
                                   "x-enumNames": [
                                     "None",
                                     "PriceChanged",
                                     "PriceChangedLow",
                                     "PriceChangedHigh",
                                     "MissingMandatoryFields",
                                     "InvalidDataFormat",
                                     "EventsOrMarketsCannotBeCombined",
                                     "SelectionsFromSameMarket",
                                     "MultipleSelectionsInEvent",
                                     "DuplicateTeamIds",
                                     "EventNotFound",
                                     "EventNotActive",
                                     "MarketNotFound",
                                     "MarketNotActive",
                                     "SelectionNotFound",
                                     "SelectionNotActive",
                                     "AdminConsoleError",
                                     "EventLimitExceeded",
                                     "MarketLimitExceeded",
                                     "BetLimitExceeded",
                                     "PayoutLimitExceeded",
                                     "EventVarLimitExceeded",
                                     "MarketVarLimitExceeded",
                                     "BetVarLimitExceeded",
                                     "PayoutVarLimitExceeded",
                                     "TooHighOdds",
                                     "SportRestricted",
                                     "EventMarkedStale",
                                     "ResponseTimeoutExceeded",
                                     "ExoticSelectionInvalidFormat",
                                     "ExoticTypeNotMatchWithMarket",
                                     "ExoticsMultibetNotSupported",
                                     "SelectionLimitExceeded",
                                     "SelectionVarLimitExceeded",
                                     "MarketIsNotFixedExotics",
                                     "FixedExoticsPriceNotFoundBySelection",
                                     "FixedExoticsSelectionContainsMultipleCombinations",
                                     "PlayerBetLimitExceeded",
                                     "StartingPriceMarketLive",
                                     "StartingPriceMultibetNotSupported",
                                     "StartingPriceMarketMatch",
                                     "NullBetPriceIsNotSupportedForThisMarket",
                                     "CombinationContainsScratchedCompetitorNumber",
                                     "BetBuilderNotSupported",
                                     "UnexpectedError"
                                   ],
                                   "x-enumDescriptions": [
                                     "Checks passed",
                                     "Price Changed",
                                     "Price Changed Low",
                                     "Price Changed High",
                                     "Missing Mandatory Fields",
                                     "Invalid Data Format",
                                     "Events Or Markets Cannot Be Combined",
                                     "Selections From Same Market",
                                     "Multiple Selections In Event",
                                     "Duplicate Team Ids",
                                     "Event Not Found",
                                     "Event Not Active",
                                     "Market Not Found",
                                     "Market Not Active",
                                     "Selection Not Found",
                                     "Selection Not Active",
                                     "Admin Console Error",
                                     "Event Limit Exceeded",
                                     "Market Limit Exceeded",
                                     "Bet Limit Exceeded",
                                     "Payout Limit Exceeded",
                                     "Event CCF Limit Exceeded",
                                     "Market CCF Limit Exceeded",
                                     "Bet CCF Limit Exceeded",
                                     "Payout CCF Limit Exceeded",
                                     "Total price was too large",
                                     "Sport restricted for placing bets",
                                     "Event has not received any updates from source for too long and bets cannot be accepted for it",
                                     "Bet placement response timed out",
                                     "Exotics selection has invalid format",
                                     "Exotics selection type does not match with market type",
                                     "Exotics multibet is not supported",
                                     "Prematch selection Limit Exceeded",
                                     "Prematch selection CCF Limit Exceeded",
                                     "Requested market is not Fixed Exotics market",
                                     "The fixed exotics price was not found by selection",
                                     "Multiple combinations are not supported for fixed exotics",
                                     "Liability is over player bet liability limit",
                                     "Starting price market went live",
                                     "Starting price multibet is not supported",
                                     "Starting price market bets should not contain price",
                                     "Null bet price is not supported for this market",
                                     "Racing selection contains scratched competitor",
                                     "Bet Builder is not supported",
                                     "Something went wrong"
                                   ]
                                 },
                                 "betItem": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.BetPlacement.BetItemViewModel"
                                     }
                                   ],
                                   "description": "Rejected bet item",
                                   "nullable": true
                                 },
                                 "maxBet": {
                                   "type": "number",
                                   "description": "Maximum allowed bet amount",
                                   "format": "double",
                                   "nullable": true
                                 },
                                 "message": {
                                   "type": "string",
                                   "description": "Detailed information about rejection cause",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "Marlin.SportsbetApi.Host.Models.Schema.MarketTypeViewModel": {
                               "type": "object",
                               "properties": {
                                 "id": {
                                   "type": "integer",
                                   "format": "int64"
                                 },
                                 "name": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "sportId": {
                                   "type": "integer",
                                   "format": "int64",
                                   "nullable": true
                                 },
                                 "sportName": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "groupId": {
                                   "type": "integer",
                                   "format": "int64",
                                   "nullable": true
                                 },
                                 "groupName": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "isMultilineMarket": {
                                   "type": "boolean"
                                 },
                                 "isFreeText": {
                                   "type": "boolean"
                                 }
                               },
                               "additionalProperties": false,
                               "description": "Represents Market Type."
                             },
                             "Marlin.SportsbetApi.Host.Models.Schema.SportPeriodViewModel": {
                               "type": "object",
                               "properties": {
                                 "id": {
                                   "type": "integer",
                                   "format": "int64"
                                 },
                                 "name": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "sportId": {
                                   "type": "integer",
                                   "format": "int64"
                                 }
                               },
                               "additionalProperties": false
                             },
                             "Marlin.SportsbetApi.Host.Models.Schema.SportViewModel": {
                               "type": "object",
                               "properties": {
                                 "id": {
                                   "type": "integer",
                                   "format": "int64"
                                 },
                                 "name": {
                                   "type": "string",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "Marlin.SportsbetApi.Host.Models.Translations.EntityTranslationsViewModel": {
                               "type": "object",
                               "properties": {
                                 "id": {
                                   "type": "integer",
                                   "format": "int64"
                                 },
                                 "translations": {
                                   "type": "object",
                                   "additionalProperties": {
                                     "type": "string",
                                     "nullable": true
                                   },
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "Marlin.SportsbetApi.Host.Models.Translations.EventTranslationsViewModel": {
                               "type": "object",
                               "properties": {
                                 "eventTranslation": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.EntityTranslationsViewModel"
                                     }
                                   ],
                                   "nullable": true
                                 },
                                 "sport": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.EntityTranslationsViewModel"
                                     }
                                   ],
                                   "nullable": true
                                 },
                                 "region": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.EntityTranslationsViewModel"
                                     }
                                   ],
                                   "nullable": true
                                 },
                                 "league": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.EntityTranslationsViewModel"
                                     }
                                   ],
                                   "nullable": true
                                 },
                                 "competitors": {
                                   "type": "array",
                                   "items": {
                                     "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.EntityTranslationsViewModel"
                                   },
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "Marlin.SportsbetApi.Host.Models.Translations.MarketTranslationViewModel": {
                               "type": "object",
                               "properties": {
                                 "market": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.TranslationViewModel"
                                     }
                                   ],
                                   "nullable": true
                                 },
                                 "selections": {
                                   "type": "array",
                                   "items": {
                                     "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.TranslationViewModel"
                                   },
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "Marlin.SportsbetApi.Host.Models.Translations.TranslatedMarketViewModel": {
                               "type": "object",
                               "properties": {
                                 "marketTranslations": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.EntityTranslationsViewModel"
                                     }
                                   ],
                                   "nullable": true
                                 },
                                 "selections": {
                                   "type": "array",
                                   "items": {
                                     "$ref": "#/components/schemas/Marlin.SportsbetApi.Host.Models.Translations.EntityTranslationsViewModel"
                                   },
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "Marlin.SportsbetApi.Host.Models.Translations.TranslationViewModel": {
                               "type": "object",
                               "properties": {
                                 "id": {
                                   "type": "integer",
                                   "format": "int64"
                                 },
                                 "language": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "translation": {
                                   "type": "string",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "Marlin.SportsbetApi.Metadata.Model.EventMetadataViewModel": {
                               "type": "object",
                               "properties": {
                                 "eventId": {
                                   "type": "integer",
                                   "format": "int64"
                                 },
                                 "sportId": {
                                   "type": "integer",
                                   "format": "int64"
                                 },
                                 "dataType": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "version": {
                                   "type": "integer",
                                   "format": "int64",
                                   "nullable": true
                                 },
                                 "data": {
                                   "nullable": true
                                 },
                                 "creationTime": {
                                   "type": "string",
                                   "format": "date-time"
                                 }
                               },
                               "additionalProperties": false
                             },
                             "Marlin.SportsbetApi.Models.FeedFactory.Delta.BetCancelDeltaViewModel": {
                               "allOf": [
                                 {
                                   "$ref": "#/components/schemas/Marlin.SportsbetApi.Models.FeedFactory.Delta.DeltaViewModel"
                                 },
                                 {
                                   "type": "object",
                                   "properties": {
                                     "marketId": {
                                       "type": "integer",
                                       "description": "Market bets on which should be voided",
                                       "format": "int64"
                                     },
                                     "from": {
                                       "type": "string",
                                       "description": "Time from which bets should be voided. Equals DateTime.MinValue if no lower bound provided",
                                       "format": "date-time"
                                     },
                                     "to": {
                                       "type": "string",
                                       "description": "Time to which bets should be voided. Equals DateTime.MaxValue if no upper bound provided",
                                       "format": "date-time"
                                     },
                                     "isRolledBack": {
                                       "type": "boolean",
                                       "description": "Flag indicating whether current Delta rollback previous cancellation for this Market and lying within same From-To range"
                                     }
                                   },
                                   "additionalProperties": false
                                 }
                               ],
                               "description": "Bets placed within period of time must be voided"
                             },
                             "Marlin.SportsbetApi.Models.FeedFactory.Delta.DeltaViewModel": {
                               "required": [
                                 "typeName"
                               ],
                               "type": "object",
                               "properties": {
                                 "type": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Enums.DeltaType"
                                     }
                                   ],
                                   "description": "Type of delta\n\n0 = None (Delta type is invalid)\n\n1 = EventStatusChanged (Event status or trading status changed)\n\n2 = EventChanged (Event added / Event info changed)\n\n3 = MarketChanged (Market added / Odds changed)\n\n4 = ScoreboardChanged (Score changed)\n\n5 = BetCancel (Bets canceled)\n\n6 = EventSnapshot (Event snapshot delta)\n\n7 = MarketsChanged (Markets added / Odds changed)",
                                   "x-enumNames": [
                                     "None",
                                     "EventStatusChanged",
                                     "EventChanged",
                                     "MarketChanged",
                                     "ScoreboardChanged",
                                     "BetCancel",
                                     "EventSnapshot",
                                     "MarketsChanged"
                                   ],
                                   "x-enumDescriptions": [
                                     "Delta type is invalid",
                                     "Event status or trading status changed",
                                     "Event added / Event info changed",
                                     "Market added / Odds changed",
                                     "Score changed",
                                     "Bets canceled",
                                     "Event snapshot delta",
                                     "Markets added / Odds changed"
                                   ]
                                 },
                                 "eventId": {
                                   "type": "integer",
                                   "description": "Id of changed event",
                                   "format": "int64"
                                 },
                                 "version": {
                                   "type": "integer",
                                   "description": "Version of feed state",
                                   "format": "int64"
                                 }
                               },
                               "additionalProperties": false,
                               "description": "Base class for delta messages",
                               "discriminator": {
                                 "propertyName": "typeName"
                               }
                             },
                             "Marlin.SportsbetApi.Models.FeedFactory.Delta.EventChangedDeltaViewModel": {
                               "allOf": [
                                 {
                                   "$ref": "#/components/schemas/Marlin.SportsbetApi.Models.FeedFactory.Delta.DeltaViewModel"
                                 },
                                 {
                                   "type": "object",
                                   "properties": {
                                     "info": {
                                       "allOf": [
                                         {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Models.FeedFactory.EventInfoViewModel"
                                         }
                                       ],
                                       "description": "Updated event info",
                                       "nullable": true
                                     }
                                   },
                                   "additionalProperties": false
                                 }
                               ],
                               "description": "Event info has been changed or new event added"
                             },
                             "Marlin.SportsbetApi.Models.FeedFactory.Delta.EventSnapshotDeltaViewModel": {
                               "allOf": [
                                 {
                                   "$ref": "#/components/schemas/Marlin.SportsbetApi.Models.FeedFactory.Delta.DeltaViewModel"
                                 },
                                 {
                                   "type": "object",
                                   "properties": {
                                     "info": {
                                       "allOf": [
                                         {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Models.FeedFactory.EventInfoViewModel"
                                         }
                                       ],
                                       "description": "Fixture info",
                                       "nullable": true
                                     },
                                     "markets": {
                                       "type": "array",
                                       "items": {
                                         "$ref": "#/components/schemas/Marlin.SportsbetApi.Models.FeedFactory.MarketViewModel"
                                       },
                                       "description": "Markets",
                                       "nullable": true
                                     },
                                     "scoreboard": {
                                       "allOf": [
                                         {
                                           "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Scoreboard.ScoreboardViewModel"
                                         }
                                       ],
                                       "description": "Scoreboard. Might be null",
                                       "nullable": true
                                     },
                                     "betCancellations": {
                                       "type": "array",
                                       "items": {
                                         "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.BetCancellationViewModel"
                                       },
                                       "description": "List of bet cancellations on this event and certain market received from BetRadar. Might be null",
                                       "nullable": true
                                     }
                                   },
                                   "additionalProperties": false
                                 }
                               ],
                               "description": "Full snapshot of event data"
                             },
                             "Marlin.SportsbetApi.Models.FeedFactory.Delta.EventStatusChangedDeltaViewModel": {
                               "allOf": [
                                 {
                                   "$ref": "#/components/schemas/Marlin.SportsbetApi.Models.FeedFactory.Delta.DeltaViewModel"
                                 },
                                 {
                                   "type": "object",
                                   "properties": {
                                     "status": {
                                       "allOf": [
                                         {
                                           "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Enums.EventStatus"
                                         }
                                       ],
                                       "description": "Event status",
                                       "x-enumNames": [
                                         "Unknown",
                                         "Planned",
                                         "SoonInPlay",
                                         "Live",
                                         "Completed",
                                         "Cancelled",
                                         "CoverageLost",
                                         "Closed",
                                         "Suspended",
                                         "Postponed",
                                         "Abandoned"
                                       ],
                                       "x-enumDescriptions": [
                                         "Event status is unknown",
                                         "Event is planned",
                                         "Event is almost started",
                                         "Event is in play",
                                         "Event finished",
                                         "Event is cancelled",
                                         "Lost updates from provider",
                                         "Event closed end will not be reopened",
                                         "Event suspended and might be re opened",
                                         "Event postponed",
                                         "Event abandoned"
                                       ]
                                     },
                                     "tradingStatus": {
                                       "allOf": [
                                         {
                                           "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Enums.TradingStatus"
                                         }
                                       ],
                                       "description": "Updated status of event trading",
                                       "x-enumNames": [
                                         "Open",
                                         "Suspended",
                                         "Closed",
                                         "Inactive"
                                       ],
                                       "x-enumDescriptions": [
                                         "Trading is active",
                                         "Trading is suspended",
                                         "Trading is stopped",
                                         "Trading is not configured"
                                       ]
                                     }
                                   },
                                   "additionalProperties": false
                                 }
                               ],
                               "description": "Event status or trading status changed"
                             },
                             "Marlin.SportsbetApi.Models.FeedFactory.Delta.MarketChangedDeltaViewModel": {
                               "allOf": [
                                 {
                                   "$ref": "#/components/schemas/Marlin.SportsbetApi.Models.FeedFactory.Delta.DeltaViewModel"
                                 },
                                 {
                                   "type": "object",
                                   "properties": {
                                     "market": {
                                       "allOf": [
                                         {
                                           "$ref": "#/components/schemas/Marlin.SportsbetApi.Models.FeedFactory.MarketViewModel"
                                         }
                                       ],
                                       "description": "Updated market",
                                       "nullable": true
                                     }
                                   },
                                   "additionalProperties": false
                                 }
                               ],
                               "description": "New market added or odds changed"
                             },
                             "Marlin.SportsbetApi.Models.FeedFactory.Delta.ScoreboardChangedDeltaViewModel": {
                               "allOf": [
                                 {
                                   "$ref": "#/components/schemas/Marlin.SportsbetApi.Models.FeedFactory.Delta.DeltaViewModel"
                                 },
                                 {
                                   "type": "object",
                                   "properties": {
                                     "scoreboard": {
                                       "allOf": [
                                         {
                                           "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Scoreboard.ScoreboardViewModel"
                                         }
                                       ],
                                       "description": "Scoreboard of event",
                                       "nullable": true
                                     }
                                   },
                                   "additionalProperties": false
                                 }
                               ],
                               "description": "Event score or time changed"
                             },
                             "Marlin.SportsbetApi.Models.FeedFactory.EventInfoViewModel": {
                               "type": "object",
                               "properties": {
                                 "id": {
                                   "type": "integer",
                                   "description": "Event id",
                                   "format": "int64"
                                 },
                                 "status": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Enums.EventStatus"
                                     }
                                   ],
                                   "description": "Event status\n\n0 = Unknown (Event status is unknown)\n\n1 = Planned (Event is planned)\n\n2 = SoonInPlay (Event is almost started)\n\n3 = Live (Event is in play)\n\n4 = Completed (Event finished)\n\n5 = Cancelled (Event is cancelled)\n\n6 = CoverageLost (Lost updates from provider)\n\n7 = Closed (Event closed end will not be reopened)\n\n8 = Suspended (Event suspended and might be re opened)\n\n9 = Postponed (Event postponed)\n\n10 = Abandoned (Event abandoned)",
                                   "x-enumNames": [
                                     "Unknown",
                                     "Planned",
                                     "SoonInPlay",
                                     "Live",
                                     "Completed",
                                     "Cancelled",
                                     "CoverageLost",
                                     "Closed",
                                     "Suspended",
                                     "Postponed",
                                     "Abandoned"
                                   ],
                                   "x-enumDescriptions": [
                                     "Event status is unknown",
                                     "Event is planned",
                                     "Event is almost started",
                                     "Event is in play",
                                     "Event finished",
                                     "Event is cancelled",
                                     "Lost updates from provider",
                                     "Event closed end will not be reopened",
                                     "Event suspended and might be re opened",
                                     "Event postponed",
                                     "Event abandoned"
                                   ]
                                 },
                                 "tradingStatus": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Enums.TradingStatus"
                                     }
                                   ],
                                   "description": "Status of trading\n\n0 = Open (Trading is active)\n\n1 = Suspended (Trading is suspended)\n\n2 = Closed (Trading is stopped)\n\n3 = Inactive (Trading is not configured)",
                                   "x-enumNames": [
                                     "Open",
                                     "Suspended",
                                     "Closed",
                                     "Inactive"
                                   ],
                                   "x-enumDescriptions": [
                                     "Trading is active",
                                     "Trading is suspended",
                                     "Trading is stopped",
                                     "Trading is not configured"
                                   ]
                                 },
                                 "statusDescription": {
                                   "type": "string",
                                   "description": "Free text event status description(optional)",
                                   "nullable": true
                                 },
                                 "country": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/Marlin.SportsbetApi.Models.FeedFactory.ReferenceViewModel"
                                     }
                                   ],
                                   "description": "Id, Name of event's country",
                                   "nullable": true
                                 },
                                 "sport": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/Marlin.SportsbetApi.Models.FeedFactory.ReferenceViewModel"
                                     }
                                   ],
                                   "description": "Id, Name of event's sport",
                                   "nullable": true
                                 },
                                 "competition": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/Marlin.SportsbetApi.Models.FeedFactory.ReferenceViewModel"
                                     }
                                   ],
                                   "description": "Id, Name of event's competition",
                                   "nullable": true
                                 },
                                 "season": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/Marlin.SportsbetApi.Models.FeedFactory.ReferenceViewModel"
                                     }
                                   ],
                                   "description": "Id, Name of event's season. Can be null.",
                                   "nullable": true
                                 },
                                 "competitors": {
                                   "type": "array",
                                   "items": {
                                     "$ref": "#/components/schemas/Marlin.SportsbetApi.Models.FeedFactory.ReferenceViewModel"
                                   },
                                   "description": "Ids, Names of event's competitors",
                                   "nullable": true
                                 },
                                 "startTime": {
                                   "type": "string",
                                   "description": "Start time of the event",
                                   "format": "date-time",
                                   "nullable": true
                                 },
                                 "mappingInfo": {
                                   "type": "array",
                                   "items": {
                                     "$ref": "#/components/schemas/Marlin.SportsbetApi.Models.FeedFactory.MappingInfoViewModel"
                                   },
                                   "description": "Providers' ids in diffrent feed systems",
                                   "nullable": true
                                 },
                                 "isLongRunningEvent": {
                                   "type": "boolean"
                                 },
                                 "name": {
                                   "type": "string",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false,
                               "description": "Information about event (sport, country, statuses etc.)"
                             },
                             "Marlin.SportsbetApi.Models.FeedFactory.MappingInfoViewModel": {
                               "type": "object",
                               "properties": {
                                 "feedId": {
                                   "type": "integer",
                                   "description": "External feed provider Id",
                                   "format": "int32"
                                 },
                                 "id": {
                                   "type": "string",
                                   "description": "Original Id of event provided by feed",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false,
                               "description": "Represents information about external feeds"
                             },
                             "Marlin.SportsbetApi.Models.FeedFactory.MarketViewModel": {
                               "type": "object",
                               "properties": {
                                 "id": {
                                   "type": "integer",
                                   "description": "Market Id",
                                   "format": "int64"
                                 },
                                 "name": {
                                   "type": "string",
                                   "description": "Market name",
                                   "nullable": true
                                 },
                                 "feedSource": {
                                   "type": "integer",
                                   "description": "Selection provider Id",
                                   "format": "int32"
                                 },
                                 "tradingStatus": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Enums.TradingStatus"
                                     }
                                   ],
                                   "description": "Market trading status\n\n0 = Open (Trading is active)\n\n1 = Suspended (Trading is suspended)\n\n2 = Closed (Trading is stopped)\n\n3 = Inactive (Trading is not configured)",
                                   "x-enumNames": [
                                     "Open",
                                     "Suspended",
                                     "Closed",
                                     "Inactive"
                                   ],
                                   "x-enumDescriptions": [
                                     "Trading is active",
                                     "Trading is suspended",
                                     "Trading is stopped",
                                     "Trading is not configured"
                                   ]
                                 },
                                 "selections": {
                                   "type": "array",
                                   "items": {
                                     "$ref": "#/components/schemas/Marlin.SportsbetApi.Models.FeedFactory.SelectionViewModel"
                                   },
                                   "description": "Market selections",
                                   "nullable": true
                                 },
                                 "externalId": {
                                   "type": "string",
                                   "description": "Provider's external id of the market",
                                   "nullable": true
                                 },
                                 "type": {
                                   "type": "integer",
                                   "description": "Market type Id",
                                   "format": "int64"
                                 },
                                 "isBestLine": {
                                   "type": "boolean",
                                   "description": "Is the main market?"
                                 },
                                 "parameters": {
                                   "type": "object",
                                   "additionalProperties": {
                                     "type": "string"
                                   },
                                   "description": "Parameters of market. i.e. hcp = 2.5, total=1.5",
                                   "nullable": true
                                 },
                                 "additionalInfo": {
                                   "type": "object",
                                   "additionalProperties": {
                                     "type": "string"
                                   },
                                   "description": "Additional market information (like score = 1:0)",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false,
                               "description": "Represents market data"
                             },
                             "Marlin.SportsbetApi.Models.FeedFactory.ReferenceViewModel": {
                               "type": "object",
                               "properties": {
                                 "id": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "name": {
                                   "type": "string",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "Marlin.SportsbetApi.Models.FeedFactory.SelectionViewModel": {
                               "type": "object",
                               "properties": {
                                 "id": {
                                   "type": "integer",
                                   "description": "Selection Id",
                                   "format": "int64"
                                 },
                                 "feedSource": {
                                   "type": "integer",
                                   "description": "Selection provider Id",
                                   "format": "int32"
                                 },
                                 "name": {
                                   "type": "string",
                                   "description": "Selection name",
                                   "nullable": true
                                 },
                                 "probability": {
                                   "type": "number",
                                   "description": "Selection probability",
                                   "format": "float",
                                   "nullable": true
                                 },
                                 "price": {
                                   "type": "number",
                                   "description": "Selection odds",
                                   "format": "float",
                                   "nullable": true
                                 },
                                 "originalPrice": {
                                   "type": "number",
                                   "description": "Selection odds",
                                   "format": "float",
                                   "nullable": true
                                 },
                                 "status": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Enums.TradingStatus"
                                     }
                                   ],
                                   "description": "Trading selection status\n\n0 = Open (Trading is active)\n\n1 = Suspended (Trading is suspended)\n\n2 = Closed (Trading is stopped)\n\n3 = Inactive (Trading is not configured)",
                                   "x-enumNames": [
                                     "Open",
                                     "Suspended",
                                     "Closed",
                                     "Inactive"
                                   ],
                                   "x-enumDescriptions": [
                                     "Trading is active",
                                     "Trading is suspended",
                                     "Trading is stopped",
                                     "Trading is not configured"
                                   ]
                                 },
                                 "settlement": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Enums.SettlementResult"
                                     }
                                   ],
                                   "description": "Settlement result of the selection\n\n0 = None (Selection is not settled)\n\n1 = Refund (Refund all bets)\n\n2 = Lose (Selection is lost)\n\n3 = Win (Selection is won)\n\n4 = HalfLose (Selection is half lost)\n\n5 = HalfWin (Selection is half won)",
                                   "x-enumNames": [
                                     "None",
                                     "Refund",
                                     "Lose",
                                     "Win",
                                     "HalfLose",
                                     "HalfWin"
                                   ],
                                   "x-enumDescriptions": [
                                     "Selection is not settled",
                                     "Refund all bets",
                                     "Selection is lost",
                                     "Selection is won",
                                     "Selection is half lost",
                                     "Selection is half won"
                                   ]
                                 },
                                 "deadHeatFactor": {
                                   "type": "number",
                                   "description": "Dead heat factor is a floating point number used when there is a tie in outrights",
                                   "format": "double",
                                   "nullable": true
                                 },
                                 "extraParameters": {
                                   "type": "object",
                                   "additionalProperties": {
                                     "type": "string"
                                   },
                                   "description": "Additional selection information",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false,
                               "description": "Represents market selection information"
                             },
                             "Marlin.SportsbetApi.Models.Settlement.CashOutViewModel": {
                               "required": [
                                 "cashOutAmount"
                               ],
                               "type": "object",
                               "properties": {
                                 "cashOutAmount": {
                                   "type": "number",
                                   "description": "Cash out amount",
                                   "format": "double"
                                 }
                               },
                               "additionalProperties": false,
                               "description": "CashOut request model"
                             },
                             "Marlin.SportsbetApi.Models.Settlement.EventInfoViewModel": {
                               "type": "object",
                               "properties": {
                                 "id": {
                                   "type": "integer",
                                   "description": "Id of the event.",
                                   "format": "int64"
                                 },
                                 "name": {
                                   "type": "string",
                                   "description": "Name of the event.",
                                   "nullable": true
                                 },
                                 "isLive": {
                                   "type": "boolean",
                                   "description": "Whether event is live or prematch."
                                 },
                                 "startTime": {
                                   "type": "string",
                                   "description": "Event start time (if available).",
                                   "format": "date-time",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false,
                               "description": "Represents event information for bet item."
                             },
                             "Marlin.SportsbetApi.Models.Settlement.MarketViewModel": {
                               "type": "object",
                               "properties": {
                                 "id": {
                                   "type": "integer",
                                   "description": "Id of the market for the associated bet item.",
                                   "format": "int64"
                                 },
                                 "type": {
                                   "type": "integer",
                                   "description": "Type id of the market for the associated bet item.",
                                   "format": "int64"
                                 },
                                 "name": {
                                   "type": "string",
                                   "description": "Name of the market for the associated bet item.",
                                   "nullable": true
                                 },
                                 "properties": {
                                   "type": "object",
                                   "additionalProperties": {
                                     "type": "string"
                                   },
                                   "description": "Extra properties of the market for the associated bet item.",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false,
                               "description": "Represents market information for bet item."
                             },
                             "Marlin.SportsbetApi.Models.Settlement.SettledBetItemViewModel": {
                               "type": "object",
                               "properties": {
                                 "event": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/Marlin.SportsbetApi.Models.Settlement.EventInfoViewModel"
                                     }
                                   ],
                                   "description": "Event associated with the bet item.",
                                   "nullable": true
                                 },
                                 "market": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/Marlin.SportsbetApi.Models.Settlement.MarketViewModel"
                                     }
                                   ],
                                   "description": "Market associated with the bet item.",
                                   "nullable": true
                                 },
                                 "selectionId": {
                                   "type": "integer",
                                   "description": "Id of the selection to which the bet item was placed.",
                                   "format": "int64"
                                 },
                                 "competitionId": {
                                   "type": "integer",
                                   "description": "Id of the competition of the associated event.",
                                   "format": "int64"
                                 },
                                 "countryId": {
                                   "type": "integer",
                                   "description": "Id of the country of the associated event.",
                                   "format": "int64"
                                 },
                                 "sportId": {
                                   "type": "integer",
                                   "description": "Id of the sport of the associated event.",
                                   "format": "int64"
                                 },
                                 "settlement": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/Marlin.SportsbetApi.Models.Settlement.SettlementInfoViewModel"
                                     }
                                   ],
                                   "description": "Bet item settlement details.",
                                   "nullable": true
                                 },
                                 "price": {
                                   "type": "number",
                                   "description": "Original price for which the bet was placed.",
                                   "format": "double",
                                   "nullable": true
                                 },
                                 "limitsRatingId": {
                                   "type": "integer",
                                   "description": "Limit id for which the bet was placed.",
                                   "format": "int64",
                                   "nullable": true
                                 },
                                 "limitsFactor": {
                                   "type": "number",
                                   "description": "Factor of the corresponding limit id.",
                                   "format": "double"
                                 },
                                 "hasLineDeviation": {
                                   "type": "boolean",
                                   "description": "Gets whether bet has live deviation."
                                 },
                                 "isOutright": {
                                   "type": "boolean",
                                   "description": "Gets whether bet was placed on outright event."
                                 },
                                 "version": {
                                   "type": "integer",
                                   "description": "Gets version of the bet item.",
                                   "format": "int32"
                                 },
                                 "deadHeatFactor": {
                                   "type": "number",
                                   "description": "Gets dead heat factor of the bet item.",
                                   "format": "double",
                                   "nullable": true
                                 },
                                 "parameters": {
                                   "type": "object",
                                   "additionalProperties": {
                                     "type": "string"
                                   },
                                   "description": "Gets parameters of the bet item selection.",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false,
                               "description": "Represents a single bet item."
                             },
                             "Marlin.SportsbetApi.Models.Settlement.SettledBetViewModelV2": {
                               "type": "object",
                               "properties": {
                                 "betId": {
                                   "type": "string",
                                   "description": "Original bet id.",
                                   "nullable": true
                                 },
                                 "betSettlement": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/Marlin.SportsbetApi.Models.Settlement.SettlementResult"
                                     }
                                   ],
                                   "description": "Settlement of the bet.\n\n0 = None\n\n1 = Win\n\n2 = Lose\n\n3 = Refund\n\n4 = HalfWin\n\n5 = HalfLose\n\n6 = CashOut",
                                   "x-enumNames": [
                                     "None",
                                     "Win",
                                     "Lose",
                                     "Refund",
                                     "HalfWin",
                                     "HalfLose",
                                     "CashOut"
                                   ],
                                   "x-enumDescriptions": [
                                     "",
                                     "",
                                     "",
                                     "",
                                     "",
                                     "",
                                     ""
                                   ]
                                 },
                                 "payout": {
                                   "type": "number",
                                   "description": "Payout calculated from initial amount and settlement.",
                                   "format": "double"
                                 },
                                 "payoutMultiplier": {
                                   "type": "number",
                                   "description": "Multiplier which was applied to the payout.",
                                   "format": "double"
                                 },
                                 "isCancelled": {
                                   "type": "boolean",
                                   "description": "Gets whether bet was cancelled or not."
                                 },
                                 "selections": {
                                   "type": "array",
                                   "items": {
                                     "$ref": "#/components/schemas/Marlin.SportsbetApi.Models.Settlement.SettledBetItemViewModel"
                                   },
                                   "description": "List of bet items for which the bet was places.",
                                   "nullable": true
                                 },
                                 "amount": {
                                   "type": "number",
                                   "description": "Initial amount of the bet.",
                                   "format": "double"
                                 },
                                 "partialRefund": {
                                   "type": "number",
                                   "description": "Amount partially refunded (specific to exotic bets).",
                                   "format": "double"
                                 },
                                 "isRejected": {
                                   "type": "boolean",
                                   "description": "Bet rejected flag."
                                 }
                               },
                               "additionalProperties": false,
                               "description": "Represents message about bet settlement."
                             },
                             "Marlin.SportsbetApi.Models.Settlement.SettlementInfoViewModel": {
                               "type": "object",
                               "properties": {
                                 "settlementResult": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/Marlin.SportsbetApi.Models.Settlement.SettlementResult"
                                     }
                                   ],
                                   "description": "Settlement result of the bet item.\n\n0 = None\n\n1 = Win\n\n2 = Lose\n\n3 = Refund\n\n4 = HalfWin\n\n5 = HalfLose\n\n6 = CashOut",
                                   "x-enumNames": [
                                     "None",
                                     "Win",
                                     "Lose",
                                     "Refund",
                                     "HalfWin",
                                     "HalfLose",
                                     "CashOut"
                                   ],
                                   "x-enumDescriptions": [
                                     "",
                                     "",
                                     "",
                                     "",
                                     "",
                                     "",
                                     ""
                                   ]
                                 },
                                 "settlementTime": {
                                   "type": "string",
                                   "description": "At what time did settlement happen.",
                                   "format": "date-time",
                                   "nullable": true
                                 },
                                 "settlementVersion": {
                                   "type": "integer",
                                   "description": "Version of the settlement in case if bet was resettled.",
                                   "format": "int32"
                                 }
                               },
                               "additionalProperties": false,
                               "description": "Contains information about bet item settlement."
                             },
                             "Marlin.SportsbetApi.Models.Settlement.SettlementResult": {
                               "enum": [
                                 0,
                                 1,
                                 2,
                                 3,
                                 4,
                                 5,
                                 6
                               ],
                               "type": "integer",
                               "description": "Settlement result of the bet.\n\n0 = None\n\n1 = Win\n\n2 = Lose\n\n3 = Refund\n\n4 = HalfWin\n\n5 = HalfLose\n\n6 = CashOut",
                               "format": "int32",
                               "x-enumNames": [
                                 "None",
                                 "Win",
                                 "Lose",
                                 "Refund",
                                 "HalfWin",
                                 "HalfLose",
                                 "CashOut"
                               ],
                               "x-enumDescriptions": [
                                 "",
                                 "",
                                 "",
                                 "",
                                 "",
                                 "",
                                 ""
                               ]
                             },
                             "MatchMetadata.ServerInformation": {
                               "type": "object",
                               "properties": {
                                 "phaseName": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "player": {
                                   "type": "string",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "MatchMetadata.Statistic.BasketballMatchStatistic": {
                               "type": "object",
                               "properties": {
                                 "statusDetail": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/MatchMetadata.Statistic.StatusDetail"
                                     }
                                   ],
                                   "nullable": true
                                 },
                                 "homePoint2": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "awayPoint2": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "homePoint3": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "awayPoint3": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "homeTimeout": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "awayTimeout": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "homeFoul": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "awayFoul": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "homeFreeThrow": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "awayFreeThrow": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "MatchMetadata.Statistic.FootballMatchStatistic": {
                               "type": "object",
                               "properties": {
                                 "statusDetail": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/MatchMetadata.Statistic.StatusDetail"
                                     }
                                   ],
                                   "nullable": true
                                 },
                                 "homeOnTarget": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "awayOnTarget": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "homeOffTarget": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "awayOffTarget": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "homeAttacks": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "awayAttacks": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "homeDangerousAttacks": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "awayDangerousAttacks": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "homeSubstitutions": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "awaySubstitutions": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "homeOffsides": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "awayOffsides": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "homePossession": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "awayPossession": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "homeGoalKicks": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "awayGoalKicks": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "homeThrowIns": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "awayThrowIns": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "MatchMetadata.Statistic.StatusDetail": {
                               "type": "object",
                               "properties": {
                                 "statusName": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "teamName": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "playerName": {
                                   "type": "string",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "MatchMetadata.Statistic.TennisMatchStatistic": {
                               "type": "object",
                               "properties": {
                                 "statusDetail": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/MatchMetadata.Statistic.StatusDetail"
                                     }
                                   ],
                                   "nullable": true
                                 },
                                 "homeAces": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "awayAces": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "homeDoubleFaults": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "awayDoubleFaults": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "homeWinFirstServe": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "awayWinFirstServe": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "homeBreakPointConvert": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "awayBreakPointConvert": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "MatchMetadata.Statistic.VolleyballMatchStatistic": {
                               "type": "object",
                               "properties": {
                                 "statusDetail": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/MatchMetadata.Statistic.StatusDetail"
                                     }
                                   ],
                                   "nullable": true
                                 },
                                 "homePointOnServe": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "awayPointOnServe": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "homeLongestStreak": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "awayLongestStreak": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "Microsoft.AspNetCore.Mvc.ProblemDetails": {
                               "type": "object",
                               "properties": {
                                 "type": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "title": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "status": {
                                   "type": "integer",
                                   "format": "int32",
                                   "nullable": true
                                 },
                                 "detail": {
                                   "type": "string",
                                   "nullable": true
                                 },
                                 "instance": {
                                   "type": "string",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": { }
                             },
                             "StatsCoreMetadata.StatsCoreIntegration": {
                               "type": "object",
                               "properties": {
                                 "id": {
                                   "type": "string",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false
                             },
                             "TradeArt.BetPlacement.GrainContracts.Enums.BetProcessorType": {
                               "enum": [
                                 1,
                                 2
                               ],
                               "type": "integer",
                               "description": "\n\n1 = Internal\n\n2 = Betradar",
                               "format": "int32",
                               "x-enumNames": [
                                 "Internal",
                                 "Betradar"
                               ],
                               "x-enumDescriptions": [
                                 "",
                                 ""
                               ]
                             },
                             "TradeArt.BetPlacement.GrainContracts.Enums.CancellationStatusType": {
                               "enum": [
                                 1,
                                 2
                               ],
                               "type": "integer",
                               "description": "\n\n1 = Accepted\n\n2 = Rejected",
                               "format": "int32",
                               "x-enumNames": [
                                 "Accepted",
                                 "Rejected"
                               ],
                               "x-enumDescriptions": [
                                 "",
                                 ""
                               ]
                             },
                             "TradeArt.BetPlacement.GrainContracts.Enums.RejectReason": {
                               "enum": [
                                 0,
                                 1,
                                 2,
                                 3,
                                 4,
                                 5,
                                 6,
                                 7,
                                 8,
                                 9,
                                 10,
                                 11,
                                 12,
                                 13,
                                 14,
                                 15,
                                 16,
                                 17,
                                 18,
                                 19,
                                 20,
                                 21,
                                 22,
                                 23,
                                 24,
                                 25,
                                 26,
                                 27,
                                 28,
                                 29,
                                 39,
                                 40,
                                 50,
                                 51,
                                 52,
                                 53,
                                 54,
                                 55,
                                 56,
                                 57,
                                 58,
                                 59,
                                 60,
                                 61,
                                 100
                               ],
                               "type": "integer",
                               "description": "\n\n0 = None (Checks passed)\n\n1 = PriceChanged (Price Changed)\n\n2 = PriceChangedLow (Price Changed Low)\n\n3 = PriceChangedHigh (Price Changed High)\n\n4 = MissingMandatoryFields (Missing Mandatory Fields)\n\n5 = InvalidDataFormat (Invalid Data Format)\n\n6 = EventsOrMarketsCannotBeCombined (Events Or Markets Cannot Be Combined)\n\n7 = SelectionsFromSameMarket (Selections From Same Market)\n\n8 = MultipleSelectionsInEvent (Multiple Selections In Event)\n\n9 = DuplicateTeamIds (Duplicate Team Ids)\n\n10 = EventNotFound (Event Not Found)\n\n11 = EventNotActive (Event Not Active)\n\n12 = MarketNotFound (Market Not Found)\n\n13 = MarketNotActive (Market Not Active)\n\n14 = SelectionNotFound (Selection Not Found)\n\n15 = SelectionNotActive (Selection Not Active)\n\n16 = AdminConsoleError (Admin Console Error)\n\n17 = EventLimitExceeded (Event Limit Exceeded)\n\n18 = MarketLimitExceeded (Market Limit Exceeded)\n\n19 = BetLimitExceeded (Bet Limit Exceeded)\n\n20 = PayoutLimitExceeded (Payout Limit Exceeded)\n\n21 = EventVarLimitExceeded (Event CCF Limit Exceeded)\n\n22 = MarketVarLimitExceeded (Market CCF Limit Exceeded)\n\n23 = BetVarLimitExceeded (Bet CCF Limit Exceeded)\n\n24 = PayoutVarLimitExceeded (Payout CCF Limit Exceeded)\n\n25 = TooHighOdds (Total price was too large)\n\n26 = SportRestricted (Sport restricted for placing bets)\n\n27 = EventMarkedStale (Event has not received any updates from source for too long and bets cannot be accepted for it)\n\n28 = ResponseTimeoutExceeded (Bet placement response timed out)\n\n29 = ExoticSelectionInvalidFormat (Exotics selection has invalid format)\n\n39 = ExoticTypeNotMatchWithMarket (Exotics selection type does not match with market type)\n\n40 = ExoticsMultibetNotSupported (Exotics multibet is not supported)\n\n50 = SelectionLimitExceeded (Prematch selection Limit Exceeded)\n\n51 = SelectionVarLimitExceeded (Prematch selection CCF Limit Exceeded)\n\n52 = MarketIsNotFixedExotics (Requested market is not Fixed Exotics market)\n\n53 = FixedExoticsPriceNotFoundBySelection (The fixed exotics price was not found by selection)\n\n54 = FixedExoticsSelectionContainsMultipleCombinations (Multiple combinations are not supported for fixed exotics)\n\n55 = PlayerBetLimitExceeded (Liability is over player bet liability limit)\n\n56 = StartingPriceMarketLive (Starting price market went live)\n\n57 = StartingPriceMultibetNotSupported (Starting price multibet is not supported)\n\n58 = StartingPriceMarketMatch (Starting price market bets should not contain price)\n\n59 = NullBetPriceIsNotSupportedForThisMarket (Null bet price is not supported for this market)\n\n60 = CombinationContainsScratchedCompetitorNumber (Racing selection contains scratched competitor)\n\n61 = BetBuilderNotSupported (Bet Builder is not supported)\n\n100 = UnexpectedError (Something went wrong)",
                               "format": "int32",
                               "x-enumNames": [
                                 "None",
                                 "PriceChanged",
                                 "PriceChangedLow",
                                 "PriceChangedHigh",
                                 "MissingMandatoryFields",
                                 "InvalidDataFormat",
                                 "EventsOrMarketsCannotBeCombined",
                                 "SelectionsFromSameMarket",
                                 "MultipleSelectionsInEvent",
                                 "DuplicateTeamIds",
                                 "EventNotFound",
                                 "EventNotActive",
                                 "MarketNotFound",
                                 "MarketNotActive",
                                 "SelectionNotFound",
                                 "SelectionNotActive",
                                 "AdminConsoleError",
                                 "EventLimitExceeded",
                                 "MarketLimitExceeded",
                                 "BetLimitExceeded",
                                 "PayoutLimitExceeded",
                                 "EventVarLimitExceeded",
                                 "MarketVarLimitExceeded",
                                 "BetVarLimitExceeded",
                                 "PayoutVarLimitExceeded",
                                 "TooHighOdds",
                                 "SportRestricted",
                                 "EventMarkedStale",
                                 "ResponseTimeoutExceeded",
                                 "ExoticSelectionInvalidFormat",
                                 "ExoticTypeNotMatchWithMarket",
                                 "ExoticsMultibetNotSupported",
                                 "SelectionLimitExceeded",
                                 "SelectionVarLimitExceeded",
                                 "MarketIsNotFixedExotics",
                                 "FixedExoticsPriceNotFoundBySelection",
                                 "FixedExoticsSelectionContainsMultipleCombinations",
                                 "PlayerBetLimitExceeded",
                                 "StartingPriceMarketLive",
                                 "StartingPriceMultibetNotSupported",
                                 "StartingPriceMarketMatch",
                                 "NullBetPriceIsNotSupportedForThisMarket",
                                 "CombinationContainsScratchedCompetitorNumber",
                                 "BetBuilderNotSupported",
                                 "UnexpectedError"
                               ],
                               "x-enumDescriptions": [
                                 "Checks passed",
                                 "Price Changed",
                                 "Price Changed Low",
                                 "Price Changed High",
                                 "Missing Mandatory Fields",
                                 "Invalid Data Format",
                                 "Events Or Markets Cannot Be Combined",
                                 "Selections From Same Market",
                                 "Multiple Selections In Event",
                                 "Duplicate Team Ids",
                                 "Event Not Found",
                                 "Event Not Active",
                                 "Market Not Found",
                                 "Market Not Active",
                                 "Selection Not Found",
                                 "Selection Not Active",
                                 "Admin Console Error",
                                 "Event Limit Exceeded",
                                 "Market Limit Exceeded",
                                 "Bet Limit Exceeded",
                                 "Payout Limit Exceeded",
                                 "Event CCF Limit Exceeded",
                                 "Market CCF Limit Exceeded",
                                 "Bet CCF Limit Exceeded",
                                 "Payout CCF Limit Exceeded",
                                 "Total price was too large",
                                 "Sport restricted for placing bets",
                                 "Event has not received any updates from source for too long and bets cannot be accepted for it",
                                 "Bet placement response timed out",
                                 "Exotics selection has invalid format",
                                 "Exotics selection type does not match with market type",
                                 "Exotics multibet is not supported",
                                 "Prematch selection Limit Exceeded",
                                 "Prematch selection CCF Limit Exceeded",
                                 "Requested market is not Fixed Exotics market",
                                 "The fixed exotics price was not found by selection",
                                 "Multiple combinations are not supported for fixed exotics",
                                 "Liability is over player bet liability limit",
                                 "Starting price market went live",
                                 "Starting price multibet is not supported",
                                 "Starting price market bets should not contain price",
                                 "Null bet price is not supported for this market",
                                 "Racing selection contains scratched competitor",
                                 "Bet Builder is not supported",
                                 "Something went wrong"
                               ]
                             },
                             "TradeArt.TradeArtApi.Contracts.DeltaMessage.BetCancellationViewModel": {
                               "type": "object",
                               "properties": {
                                 "marketId": {
                                   "type": "integer",
                                   "description": "Affected MarketId",
                                   "format": "int64"
                                 },
                                 "from": {
                                   "type": "string",
                                   "description": "Cancel bets placed starting From this date. Equals DateTime.MinValue if no lower bound provided",
                                   "format": "date-time"
                                 },
                                 "to": {
                                   "type": "string",
                                   "description": "Cancel bets placed starting To this date. Equals DateTime.MaxValue if no upper bound provided",
                                   "format": "date-time"
                                 },
                                 "isRolledBack": {
                                   "type": "boolean",
                                   "description": "Flag pointing whether this cancellation should be rolled back"
                                 }
                               },
                               "additionalProperties": false,
                               "description": "Information about bet cancellations on certain market between From and To dates"
                             },
                             "TradeArt.TradeArtApi.Contracts.DeltaMessage.Delta.BetCancelDeltaViewModelV2": {
                               "type": "object",
                               "properties": {
                                 "type": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Enums.DeltaType"
                                     }
                                   ],
                                   "description": "Type of delta\n\n0 = None (Delta type is invalid)\n\n1 = EventStatusChanged (Event status or trading status changed)\n\n2 = EventChanged (Event added / Event info changed)\n\n3 = MarketChanged (Market added / Odds changed)\n\n4 = ScoreboardChanged (Score changed)\n\n5 = BetCancel (Bets canceled)\n\n6 = EventSnapshot (Event snapshot delta)\n\n7 = MarketsChanged (Markets added / Odds changed)",
                                   "x-enumNames": [
                                     "None",
                                     "EventStatusChanged",
                                     "EventChanged",
                                     "MarketChanged",
                                     "ScoreboardChanged",
                                     "BetCancel",
                                     "EventSnapshot",
                                     "MarketsChanged"
                                   ],
                                   "x-enumDescriptions": [
                                     "Delta type is invalid",
                                     "Event status or trading status changed",
                                     "Event added / Event info changed",
                                     "Market added / Odds changed",
                                     "Score changed",
                                     "Bets canceled",
                                     "Event snapshot delta",
                                     "Markets added / Odds changed"
                                   ]
                                 },
                                 "eventId": {
                                   "type": "integer",
                                   "description": "Id of changed event",
                                   "format": "int64"
                                 },
                                 "version": {
                                   "type": "integer",
                                   "description": "Version of feed state",
                                   "format": "int64"
                                 },
                                 "correlationId": {
                                   "type": "string",
                                   "description": "Correlation id of the message to trace back.",
                                   "nullable": true
                                 },
                                 "creationTime": {
                                   "type": "string",
                                   "description": "Captures the timestamp the dto was created.",
                                   "format": "date-time"
                                 },
                                 "sportId": {
                                   "type": "integer",
                                   "description": "Id of the Sport",
                                   "format": "int64"
                                 },
                                 "locationId": {
                                   "type": "integer",
                                   "description": "Id of the Location",
                                   "format": "int64"
                                 },
                                 "leagueId": {
                                   "type": "integer",
                                   "description": "Id of the League",
                                   "format": "int64"
                                 },
                                 "marketId": {
                                   "type": "integer",
                                   "description": "Market bets on which should be voided",
                                   "format": "int64"
                                 },
                                 "from": {
                                   "type": "string",
                                   "description": "Time from which bets should be voided. Equals DateTime.MinValue if no lower bound provided",
                                   "format": "date-time"
                                 },
                                 "to": {
                                   "type": "string",
                                   "description": "Time to which bets should be voided. Equals DateTime.MaxValue if no upper bound provided",
                                   "format": "date-time"
                                 },
                                 "isRolledBack": {
                                   "type": "boolean",
                                   "description": "Flag indicating whether current Delta rollback previous cancellation for this Market and lying within same From-To range"
                                 }
                               },
                               "additionalProperties": false,
                               "description": "Bets placed within period of time must be voided"
                             },
                             "TradeArt.TradeArtApi.Contracts.DeltaMessage.Delta.EventChangedDeltaViewModelV2": {
                               "type": "object",
                               "properties": {
                                 "type": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Enums.DeltaType"
                                     }
                                   ],
                                   "description": "Type of delta\n\n0 = None (Delta type is invalid)\n\n1 = EventStatusChanged (Event status or trading status changed)\n\n2 = EventChanged (Event added / Event info changed)\n\n3 = MarketChanged (Market added / Odds changed)\n\n4 = ScoreboardChanged (Score changed)\n\n5 = BetCancel (Bets canceled)\n\n6 = EventSnapshot (Event snapshot delta)\n\n7 = MarketsChanged (Markets added / Odds changed)",
                                   "x-enumNames": [
                                     "None",
                                     "EventStatusChanged",
                                     "EventChanged",
                                     "MarketChanged",
                                     "ScoreboardChanged",
                                     "BetCancel",
                                     "EventSnapshot",
                                     "MarketsChanged"
                                   ],
                                   "x-enumDescriptions": [
                                     "Delta type is invalid",
                                     "Event status or trading status changed",
                                     "Event added / Event info changed",
                                     "Market added / Odds changed",
                                     "Score changed",
                                     "Bets canceled",
                                     "Event snapshot delta",
                                     "Markets added / Odds changed"
                                   ]
                                 },
                                 "eventId": {
                                   "type": "integer",
                                   "description": "Id of changed event",
                                   "format": "int64"
                                 },
                                 "version": {
                                   "type": "integer",
                                   "description": "Version of feed state",
                                   "format": "int64"
                                 },
                                 "correlationId": {
                                   "type": "string",
                                   "description": "Correlation id of the message to trace back.",
                                   "nullable": true
                                 },
                                 "creationTime": {
                                   "type": "string",
                                   "description": "Captures the timestamp the dto was created.",
                                   "format": "date-time"
                                 },
                                 "sportId": {
                                   "type": "integer",
                                   "description": "Id of the Sport",
                                   "format": "int64"
                                 },
                                 "locationId": {
                                   "type": "integer",
                                   "description": "Id of the Location",
                                   "format": "int64"
                                 },
                                 "leagueId": {
                                   "type": "integer",
                                   "description": "Id of the League",
                                   "format": "int64"
                                 },
                                 "eventInfo": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.EventInfoViewModelV2"
                                     }
                                   ],
                                   "description": "Updated event info",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false,
                               "description": "Event info has been changed or new event added"
                             },
                             "TradeArt.TradeArtApi.Contracts.DeltaMessage.Delta.EventSnapshotDeltaViewModelV2": {
                               "type": "object",
                               "properties": {
                                 "type": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Enums.DeltaType"
                                     }
                                   ],
                                   "description": "Type of delta\n\n0 = None (Delta type is invalid)\n\n1 = EventStatusChanged (Event status or trading status changed)\n\n2 = EventChanged (Event added / Event info changed)\n\n3 = MarketChanged (Market added / Odds changed)\n\n4 = ScoreboardChanged (Score changed)\n\n5 = BetCancel (Bets canceled)\n\n6 = EventSnapshot (Event snapshot delta)\n\n7 = MarketsChanged (Markets added / Odds changed)",
                                   "x-enumNames": [
                                     "None",
                                     "EventStatusChanged",
                                     "EventChanged",
                                     "MarketChanged",
                                     "ScoreboardChanged",
                                     "BetCancel",
                                     "EventSnapshot",
                                     "MarketsChanged"
                                   ],
                                   "x-enumDescriptions": [
                                     "Delta type is invalid",
                                     "Event status or trading status changed",
                                     "Event added / Event info changed",
                                     "Market added / Odds changed",
                                     "Score changed",
                                     "Bets canceled",
                                     "Event snapshot delta",
                                     "Markets added / Odds changed"
                                   ]
                                 },
                                 "eventId": {
                                   "type": "integer",
                                   "description": "Id of changed event",
                                   "format": "int64"
                                 },
                                 "version": {
                                   "type": "integer",
                                   "description": "Version of feed state",
                                   "format": "int64"
                                 },
                                 "correlationId": {
                                   "type": "string",
                                   "description": "Correlation id of the message to trace back.",
                                   "nullable": true
                                 },
                                 "creationTime": {
                                   "type": "string",
                                   "description": "Captures the timestamp the dto was created.",
                                   "format": "date-time"
                                 },
                                 "sportId": {
                                   "type": "integer",
                                   "description": "Id of the Sport",
                                   "format": "int64"
                                 },
                                 "locationId": {
                                   "type": "integer",
                                   "description": "Id of the Location",
                                   "format": "int64"
                                 },
                                 "leagueId": {
                                   "type": "integer",
                                   "description": "Id of the League",
                                   "format": "int64"
                                 },
                                 "eventInfo": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.EventInfoViewModelV2"
                                     }
                                   ],
                                   "description": "Fixture info containing basic event information.",
                                   "nullable": true
                                 },
                                 "markets": {
                                   "type": "array",
                                   "items": {
                                     "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.MarketViewModelV2"
                                   },
                                   "description": "Markets of the event.",
                                   "nullable": true
                                 },
                                 "scoreboard": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Scoreboard.ScoreboardViewModel"
                                     }
                                   ],
                                   "description": "Scoreboard of the event. Can be null.",
                                   "nullable": true
                                 },
                                 "betCancellations": {
                                   "type": "array",
                                   "items": {
                                     "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.BetCancellationViewModel"
                                   },
                                   "description": "List of bet cancellations on this event and certain market received from BetRadar. Can be null.",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false,
                               "description": "Full snapshot of event data"
                             },
                             "TradeArt.TradeArtApi.Contracts.DeltaMessage.Delta.EventStatusChangedDeltaViewModelV2": {
                               "type": "object",
                               "properties": {
                                 "type": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Enums.DeltaType"
                                     }
                                   ],
                                   "description": "Type of delta\n\n0 = None (Delta type is invalid)\n\n1 = EventStatusChanged (Event status or trading status changed)\n\n2 = EventChanged (Event added / Event info changed)\n\n3 = MarketChanged (Market added / Odds changed)\n\n4 = ScoreboardChanged (Score changed)\n\n5 = BetCancel (Bets canceled)\n\n6 = EventSnapshot (Event snapshot delta)\n\n7 = MarketsChanged (Markets added / Odds changed)",
                                   "x-enumNames": [
                                     "None",
                                     "EventStatusChanged",
                                     "EventChanged",
                                     "MarketChanged",
                                     "ScoreboardChanged",
                                     "BetCancel",
                                     "EventSnapshot",
                                     "MarketsChanged"
                                   ],
                                   "x-enumDescriptions": [
                                     "Delta type is invalid",
                                     "Event status or trading status changed",
                                     "Event added / Event info changed",
                                     "Market added / Odds changed",
                                     "Score changed",
                                     "Bets canceled",
                                     "Event snapshot delta",
                                     "Markets added / Odds changed"
                                   ]
                                 },
                                 "eventId": {
                                   "type": "integer",
                                   "description": "Id of changed event",
                                   "format": "int64"
                                 },
                                 "version": {
                                   "type": "integer",
                                   "description": "Version of feed state",
                                   "format": "int64"
                                 },
                                 "correlationId": {
                                   "type": "string",
                                   "description": "Correlation id of the message to trace back.",
                                   "nullable": true
                                 },
                                 "creationTime": {
                                   "type": "string",
                                   "description": "Captures the timestamp the dto was created.",
                                   "format": "date-time"
                                 },
                                 "sportId": {
                                   "type": "integer",
                                   "description": "Id of the Sport",
                                   "format": "int64"
                                 },
                                 "locationId": {
                                   "type": "integer",
                                   "description": "Id of the Location",
                                   "format": "int64"
                                 },
                                 "leagueId": {
                                   "type": "integer",
                                   "description": "Id of the League",
                                   "format": "int64"
                                 },
                                 "status": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Enums.EventStatus"
                                     }
                                   ],
                                   "description": "Event status\n\n0 = Unknown (Event status is unknown)\n\n1 = Planned (Event is planned)\n\n2 = SoonInPlay (Event is almost started)\n\n3 = Live (Event is in play)\n\n4 = Completed (Event finished)\n\n5 = Cancelled (Event is cancelled)\n\n6 = CoverageLost (Lost updates from provider)\n\n7 = Closed (Event closed end will not be reopened)\n\n8 = Suspended (Event suspended and might be re opened)\n\n9 = Postponed (Event postponed)\n\n10 = Abandoned (Event abandoned)",
                                   "x-enumNames": [
                                     "Unknown",
                                     "Planned",
                                     "SoonInPlay",
                                     "Live",
                                     "Completed",
                                     "Cancelled",
                                     "CoverageLost",
                                     "Closed",
                                     "Suspended",
                                     "Postponed",
                                     "Abandoned"
                                   ],
                                   "x-enumDescriptions": [
                                     "Event status is unknown",
                                     "Event is planned",
                                     "Event is almost started",
                                     "Event is in play",
                                     "Event finished",
                                     "Event is cancelled",
                                     "Lost updates from provider",
                                     "Event closed end will not be reopened",
                                     "Event suspended and might be re opened",
                                     "Event postponed",
                                     "Event abandoned"
                                   ]
                                 },
                                 "tradingStatus": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Enums.TradingStatus"
                                     }
                                   ],
                                   "description": "Updated status of event trading\n\n0 = Open (Trading is active)\n\n1 = Suspended (Trading is suspended)\n\n2 = Closed (Trading is stopped)\n\n3 = Inactive (Trading is not configured)",
                                   "x-enumNames": [
                                     "Open",
                                     "Suspended",
                                     "Closed",
                                     "Inactive"
                                   ],
                                   "x-enumDescriptions": [
                                     "Trading is active",
                                     "Trading is suspended",
                                     "Trading is stopped",
                                     "Trading is not configured"
                                   ]
                                 }
                               },
                               "additionalProperties": false,
                               "description": "Event status or trading status changed"
                             },
                             "TradeArt.TradeArtApi.Contracts.DeltaMessage.Delta.MarketsChangedDeltaViewModelV2": {
                               "type": "object",
                               "properties": {
                                 "type": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Enums.DeltaType"
                                     }
                                   ],
                                   "description": "Type of delta\n\n0 = None (Delta type is invalid)\n\n1 = EventStatusChanged (Event status or trading status changed)\n\n2 = EventChanged (Event added / Event info changed)\n\n3 = MarketChanged (Market added / Odds changed)\n\n4 = ScoreboardChanged (Score changed)\n\n5 = BetCancel (Bets canceled)\n\n6 = EventSnapshot (Event snapshot delta)\n\n7 = MarketsChanged (Markets added / Odds changed)",
                                   "x-enumNames": [
                                     "None",
                                     "EventStatusChanged",
                                     "EventChanged",
                                     "MarketChanged",
                                     "ScoreboardChanged",
                                     "BetCancel",
                                     "EventSnapshot",
                                     "MarketsChanged"
                                   ],
                                   "x-enumDescriptions": [
                                     "Delta type is invalid",
                                     "Event status or trading status changed",
                                     "Event added / Event info changed",
                                     "Market added / Odds changed",
                                     "Score changed",
                                     "Bets canceled",
                                     "Event snapshot delta",
                                     "Markets added / Odds changed"
                                   ]
                                 },
                                 "eventId": {
                                   "type": "integer",
                                   "description": "Id of changed event",
                                   "format": "int64"
                                 },
                                 "version": {
                                   "type": "integer",
                                   "description": "Version of feed state",
                                   "format": "int64"
                                 },
                                 "correlationId": {
                                   "type": "string",
                                   "description": "Correlation id of the message to trace back.",
                                   "nullable": true
                                 },
                                 "creationTime": {
                                   "type": "string",
                                   "description": "Captures the timestamp the dto was created.",
                                   "format": "date-time"
                                 },
                                 "sportId": {
                                   "type": "integer",
                                   "description": "Id of the Sport",
                                   "format": "int64"
                                 },
                                 "locationId": {
                                   "type": "integer",
                                   "description": "Id of the Location",
                                   "format": "int64"
                                 },
                                 "leagueId": {
                                   "type": "integer",
                                   "description": "Id of the League",
                                   "format": "int64"
                                 },
                                 "markets": {
                                   "type": "array",
                                   "items": {
                                     "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.MarketViewModelV2"
                                   },
                                   "description": "Updated markets",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false,
                               "description": "New markets added or odds changed"
                             },
                             "TradeArt.TradeArtApi.Contracts.DeltaMessage.Delta.ScoreboardChangedDeltaViewModelV2": {
                               "type": "object",
                               "properties": {
                                 "type": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Enums.DeltaType"
                                     }
                                   ],
                                   "description": "Type of delta\n\n0 = None (Delta type is invalid)\n\n1 = EventStatusChanged (Event status or trading status changed)\n\n2 = EventChanged (Event added / Event info changed)\n\n3 = MarketChanged (Market added / Odds changed)\n\n4 = ScoreboardChanged (Score changed)\n\n5 = BetCancel (Bets canceled)\n\n6 = EventSnapshot (Event snapshot delta)\n\n7 = MarketsChanged (Markets added / Odds changed)",
                                   "x-enumNames": [
                                     "None",
                                     "EventStatusChanged",
                                     "EventChanged",
                                     "MarketChanged",
                                     "ScoreboardChanged",
                                     "BetCancel",
                                     "EventSnapshot",
                                     "MarketsChanged"
                                   ],
                                   "x-enumDescriptions": [
                                     "Delta type is invalid",
                                     "Event status or trading status changed",
                                     "Event added / Event info changed",
                                     "Market added / Odds changed",
                                     "Score changed",
                                     "Bets canceled",
                                     "Event snapshot delta",
                                     "Markets added / Odds changed"
                                   ]
                                 },
                                 "eventId": {
                                   "type": "integer",
                                   "description": "Id of changed event",
                                   "format": "int64"
                                 },
                                 "version": {
                                   "type": "integer",
                                   "description": "Version of feed state",
                                   "format": "int64"
                                 },
                                 "correlationId": {
                                   "type": "string",
                                   "description": "Correlation id of the message to trace back.",
                                   "nullable": true
                                 },
                                 "creationTime": {
                                   "type": "string",
                                   "description": "Captures the timestamp the dto was created.",
                                   "format": "date-time"
                                 },
                                 "sportId": {
                                   "type": "integer",
                                   "description": "Id of the Sport",
                                   "format": "int64"
                                 },
                                 "locationId": {
                                   "type": "integer",
                                   "description": "Id of the Location",
                                   "format": "int64"
                                 },
                                 "leagueId": {
                                   "type": "integer",
                                   "description": "Id of the League",
                                   "format": "int64"
                                 },
                                 "scoreboard": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Scoreboard.ScoreboardViewModel"
                                     }
                                   ],
                                   "description": "Scoreboard of event",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false,
                               "description": "Event score or time changed"
                             },
                             "TradeArt.TradeArtApi.Contracts.DeltaMessage.Enums.DeltaType": {
                               "enum": [
                                 0,
                                 1,
                                 2,
                                 3,
                                 4,
                                 5,
                                 6,
                                 7
                               ],
                               "type": "integer",
                               "description": "Known types of deltas\n\n0 = None (Delta type is invalid)\n\n1 = EventStatusChanged (Event status or trading status changed)\n\n2 = EventChanged (Event added / Event info changed)\n\n3 = MarketChanged (Market added / Odds changed)\n\n4 = ScoreboardChanged (Score changed)\n\n5 = BetCancel (Bets canceled)\n\n6 = EventSnapshot (Event snapshot delta)\n\n7 = MarketsChanged (Markets added / Odds changed)",
                               "format": "int32",
                               "x-enumNames": [
                                 "None",
                                 "EventStatusChanged",
                                 "EventChanged",
                                 "MarketChanged",
                                 "ScoreboardChanged",
                                 "BetCancel",
                                 "EventSnapshot",
                                 "MarketsChanged"
                               ],
                               "x-enumDescriptions": [
                                 "Delta type is invalid",
                                 "Event status or trading status changed",
                                 "Event added / Event info changed",
                                 "Market added / Odds changed",
                                 "Score changed",
                                 "Bets canceled",
                                 "Event snapshot delta",
                                 "Markets added / Odds changed"
                               ]
                             },
                             "TradeArt.TradeArtApi.Contracts.DeltaMessage.Enums.EventStatus": {
                               "enum": [
                                 0,
                                 1,
                                 2,
                                 3,
                                 4,
                                 5,
                                 6,
                                 7,
                                 8,
                                 9,
                                 10
                               ],
                               "type": "integer",
                               "description": "Status of event\n\n0 = Unknown (Event status is unknown)\n\n1 = Planned (Event is planned)\n\n2 = SoonInPlay (Event is almost started)\n\n3 = Live (Event is in play)\n\n4 = Completed (Event finished)\n\n5 = Cancelled (Event is cancelled)\n\n6 = CoverageLost (Lost updates from provider)\n\n7 = Closed (Event closed end will not be reopened)\n\n8 = Suspended (Event suspended and might be re opened)\n\n9 = Postponed (Event postponed)\n\n10 = Abandoned (Event abandoned)",
                               "format": "int32",
                               "x-enumNames": [
                                 "Unknown",
                                 "Planned",
                                 "SoonInPlay",
                                 "Live",
                                 "Completed",
                                 "Cancelled",
                                 "CoverageLost",
                                 "Closed",
                                 "Suspended",
                                 "Postponed",
                                 "Abandoned"
                               ],
                               "x-enumDescriptions": [
                                 "Event status is unknown",
                                 "Event is planned",
                                 "Event is almost started",
                                 "Event is in play",
                                 "Event finished",
                                 "Event is cancelled",
                                 "Lost updates from provider",
                                 "Event closed end will not be reopened",
                                 "Event suspended and might be re opened",
                                 "Event postponed",
                                 "Event abandoned"
                               ]
                             },
                             "TradeArt.TradeArtApi.Contracts.DeltaMessage.Enums.SettlementResult": {
                               "enum": [
                                 0,
                                 1,
                                 2,
                                 3,
                                 4,
                                 5
                               ],
                               "type": "integer",
                               "description": "Status of a selection settlement\n\n0 = None (Selection is not settled)\n\n1 = Refund (Refund all bets)\n\n2 = Lose (Selection is lost)\n\n3 = Win (Selection is won)\n\n4 = HalfLose (Selection is half lost)\n\n5 = HalfWin (Selection is half won)",
                               "format": "int32",
                               "x-enumNames": [
                                 "None",
                                 "Refund",
                                 "Lose",
                                 "Win",
                                 "HalfLose",
                                 "HalfWin"
                               ],
                               "x-enumDescriptions": [
                                 "Selection is not settled",
                                 "Refund all bets",
                                 "Selection is lost",
                                 "Selection is won",
                                 "Selection is half lost",
                                 "Selection is half won"
                               ]
                             },
                             "TradeArt.TradeArtApi.Contracts.DeltaMessage.Enums.TradingStatus": {
                               "enum": [
                                 0,
                                 1,
                                 2,
                                 3
                               ],
                               "type": "integer",
                               "description": "Status of trading\n\n0 = Open (Trading is active)\n\n1 = Suspended (Trading is suspended)\n\n2 = Closed (Trading is stopped)\n\n3 = Inactive (Trading is not configured)",
                               "format": "int32",
                               "x-enumNames": [
                                 "Open",
                                 "Suspended",
                                 "Closed",
                                 "Inactive"
                               ],
                               "x-enumDescriptions": [
                                 "Trading is active",
                                 "Trading is suspended",
                                 "Trading is stopped",
                                 "Trading is not configured"
                               ]
                             },
                             "TradeArt.TradeArtApi.Contracts.DeltaMessage.EventInfoViewModelV2": {
                               "type": "object",
                               "properties": {
                                 "id": {
                                   "type": "integer",
                                   "description": "TradeArt event id.",
                                   "format": "int64"
                                 },
                                 "name": {
                                   "type": "string",
                                   "description": "Event name.",
                                   "nullable": true
                                 },
                                 "status": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Enums.EventStatus"
                                     }
                                   ],
                                   "description": "Event status representing current status of the match.\n\n0 = Unknown (Event status is unknown)\n\n1 = Planned (Event is planned)\n\n2 = SoonInPlay (Event is almost started)\n\n3 = Live (Event is in play)\n\n4 = Completed (Event finished)\n\n5 = Cancelled (Event is cancelled)\n\n6 = CoverageLost (Lost updates from provider)\n\n7 = Closed (Event closed end will not be reopened)\n\n8 = Suspended (Event suspended and might be re opened)\n\n9 = Postponed (Event postponed)\n\n10 = Abandoned (Event abandoned)",
                                   "x-enumNames": [
                                     "Unknown",
                                     "Planned",
                                     "SoonInPlay",
                                     "Live",
                                     "Completed",
                                     "Cancelled",
                                     "CoverageLost",
                                     "Closed",
                                     "Suspended",
                                     "Postponed",
                                     "Abandoned"
                                   ],
                                   "x-enumDescriptions": [
                                     "Event status is unknown",
                                     "Event is planned",
                                     "Event is almost started",
                                     "Event is in play",
                                     "Event finished",
                                     "Event is cancelled",
                                     "Lost updates from provider",
                                     "Event closed end will not be reopened",
                                     "Event suspended and might be re opened",
                                     "Event postponed",
                                     "Event abandoned"
                                   ]
                                 },
                                 "tradingStatus": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Enums.TradingStatus"
                                     }
                                   ],
                                   "description": "Status of trading for the event showing how it should be processed.\n\n0 = Open (Trading is active)\n\n1 = Suspended (Trading is suspended)\n\n2 = Closed (Trading is stopped)\n\n3 = Inactive (Trading is not configured)",
                                   "x-enumNames": [
                                     "Open",
                                     "Suspended",
                                     "Closed",
                                     "Inactive"
                                   ],
                                   "x-enumDescriptions": [
                                     "Trading is active",
                                     "Trading is suspended",
                                     "Trading is stopped",
                                     "Trading is not configured"
                                   ]
                                 },
                                 "statusDescription": {
                                   "type": "string",
                                   "description": "Free text event status description (optional).",
                                   "nullable": true
                                 },
                                 "country": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.ReferenceViewModelV2"
                                     }
                                   ],
                                   "description": "Id and Name of event's country.",
                                   "nullable": true
                                 },
                                 "sport": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.ReferenceViewModelV2"
                                     }
                                   ],
                                   "description": "Id and Name of event's sport.",
                                   "nullable": true
                                 },
                                 "competition": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.ReferenceViewModelV2"
                                     }
                                   ],
                                   "description": "Id and Name of event's competition.",
                                   "nullable": true
                                 },
                                 "season": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.ReferenceViewModelV2"
                                     }
                                   ],
                                   "description": "Id and Name of event's season. Can be null for not-outright events.",
                                   "nullable": true
                                 },
                                 "competitors": {
                                   "type": "array",
                                   "items": {
                                     "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.ReferenceViewModelV2"
                                   },
                                   "description": "Ids and Names of event's competitors.",
                                   "nullable": true
                                 },
                                 "startTime": {
                                   "type": "string",
                                   "description": "Start time of the event. Can be null.",
                                   "format": "date-time",
                                   "nullable": true
                                 },
                                 "isOutright": {
                                   "type": "boolean",
                                   "description": "Flag which indicates that event is outright."
                                 },
                                 "customDisplayData": {
                                   "type": "object",
                                   "additionalProperties": {
                                     "type": "string"
                                   },
                                   "description": "Custom display parameters of the event",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false,
                               "description": "Information about event (sport, country, statuses etc.)"
                             },
                             "TradeArt.TradeArtApi.Contracts.DeltaMessage.MarketViewModelV2": {
                               "type": "object",
                               "properties": {
                                 "id": {
                                   "type": "integer",
                                   "description": "Market Id",
                                   "format": "int64"
                                 },
                                 "name": {
                                   "type": "string",
                                   "description": "Market name",
                                   "nullable": true
                                 },
                                 "tradingStatus": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Enums.TradingStatus"
                                     }
                                   ],
                                   "description": "Market trading status\n\n0 = Open (Trading is active)\n\n1 = Suspended (Trading is suspended)\n\n2 = Closed (Trading is stopped)\n\n3 = Inactive (Trading is not configured)",
                                   "x-enumNames": [
                                     "Open",
                                     "Suspended",
                                     "Closed",
                                     "Inactive"
                                   ],
                                   "x-enumDescriptions": [
                                     "Trading is active",
                                     "Trading is suspended",
                                     "Trading is stopped",
                                     "Trading is not configured"
                                   ]
                                 },
                                 "selections": {
                                   "type": "array",
                                   "items": {
                                     "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.SelectionViewModelV2"
                                   },
                                   "description": "Market selections",
                                   "nullable": true
                                 },
                                 "type": {
                                   "type": "integer",
                                   "description": "Market type Id",
                                   "format": "int64"
                                 },
                                 "isBestLine": {
                                   "type": "boolean",
                                   "description": "Is the main market?"
                                 },
                                 "parameters": {
                                   "type": "object",
                                   "additionalProperties": {
                                     "type": "string"
                                   },
                                   "description": "Parameters of market. i.e. hcp = 2.5, total=1.5",
                                   "nullable": true
                                 },
                                 "extraParameters": {
                                   "type": "object",
                                   "additionalProperties": {
                                     "type": "string"
                                   },
                                   "description": "Additional market information (like score = 1:0)",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false,
                               "description": "Represents market data"
                             },
                             "TradeArt.TradeArtApi.Contracts.DeltaMessage.ReferenceViewModelV2": {
                               "type": "object",
                               "properties": {
                                 "id": {
                                   "type": "integer",
                                   "description": "Id of the referenced entity",
                                   "format": "int64"
                                 },
                                 "name": {
                                   "type": "string",
                                   "description": "Name of the referenced entity",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false,
                               "description": "Contains reference data for entity"
                             },
                             "TradeArt.TradeArtApi.Contracts.DeltaMessage.Scoreboard.IncidentCountViewModel": {
                               "type": "object",
                               "properties": {
                                 "type": {
                                   "type": "integer",
                                   "description": "Incident type",
                                   "format": "int32"
                                 },
                                 "competitorPosition": {
                                   "type": "integer",
                                   "description": "Competitor position (e.g. 1 - Home, 2 - Away)",
                                   "format": "int32"
                                 },
                                 "count": {
                                   "type": "integer",
                                   "description": "Incident count",
                                   "format": "int32"
                                 }
                               },
                               "additionalProperties": false,
                               "description": "Incident count by type and competitor"
                             },
                             "TradeArt.TradeArtApi.Contracts.DeltaMessage.Scoreboard.IncidentViewModel": {
                               "type": "object",
                               "properties": {
                                 "type": {
                                   "type": "integer",
                                   "description": "Incident type",
                                   "format": "int32"
                                 },
                                 "time": {
                                   "type": "integer",
                                   "description": "Event time",
                                   "format": "int32"
                                 },
                                 "competitorPosition": {
                                   "type": "integer",
                                   "description": "Index of the competitor in the EventInfo.Competitors array",
                                   "format": "int32"
                                 }
                               },
                               "additionalProperties": false,
                               "description": "Represents incident information"
                             },
                             "TradeArt.TradeArtApi.Contracts.DeltaMessage.Scoreboard.PeriodViewModel": {
                               "type": "object",
                               "properties": {
                                 "id": {
                                   "type": "integer",
                                   "description": "Period Id",
                                   "format": "int32"
                                 },
                                 "isFinished": {
                                   "type": "boolean",
                                   "description": "Is period finished?"
                                 },
                                 "incidents": {
                                   "type": "array",
                                   "items": {
                                     "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Scoreboard.IncidentViewModel"
                                   },
                                   "description": "Incidents occurred in the period",
                                   "nullable": true
                                 },
                                 "results": {
                                   "type": "array",
                                   "items": {
                                     "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Scoreboard.ResultViewModel"
                                   },
                                   "description": "Results of the period",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false,
                               "description": "Represents period of the event"
                             },
                             "TradeArt.TradeArtApi.Contracts.DeltaMessage.Scoreboard.ResultViewModel": {
                               "type": "object",
                               "properties": {
                                 "competitorPosition": {
                                   "type": "integer",
                                   "description": "Competitor position (e.g. 1 - Home, 2 - Away)",
                                   "format": "int32"
                                 },
                                 "value": {
                                   "type": "string",
                                   "description": "Result value",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false,
                               "description": "Represents scoreboard result"
                             },
                             "TradeArt.TradeArtApi.Contracts.DeltaMessage.Scoreboard.ScoreboardViewModel": {
                               "type": "object",
                               "properties": {
                                 "time": {
                                   "type": "integer",
                                   "description": "Current time of event (in seconds)",
                                   "format": "int32"
                                 },
                                 "results": {
                                   "type": "array",
                                   "items": {
                                     "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Scoreboard.ResultViewModel"
                                   },
                                   "description": "Current event results per competitor (in seconds)",
                                   "nullable": true
                                 },
                                 "periods": {
                                   "type": "array",
                                   "items": {
                                     "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Scoreboard.PeriodViewModel"
                                   },
                                   "description": "Current event periods with results and incidents",
                                   "nullable": true
                                 },
                                 "incidents": {
                                   "type": "array",
                                   "items": {
                                     "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Scoreboard.IncidentCountViewModel"
                                   },
                                   "description": "Current event incident summary",
                                   "nullable": true
                                 },
                                 "currentPeriodId": {
                                   "type": "integer",
                                   "description": "Internal id of current period",
                                   "format": "int32"
                                 },
                                 "lastUpdateTime": {
                                   "type": "string",
                                   "description": "Last update time of the scoreboard",
                                   "format": "date-time"
                                 },
                                 "homeGameScore": {
                                   "type": "string",
                                   "description": "Current home team game score",
                                   "nullable": true
                                 },
                                 "awayGameScore": {
                                   "type": "string",
                                   "description": "Current away team game score",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false,
                               "description": "Represents event's scoreboard"
                             },
                             "TradeArt.TradeArtApi.Contracts.DeltaMessage.SelectionViewModelV2": {
                               "type": "object",
                               "properties": {
                                 "id": {
                                   "type": "integer",
                                   "description": "Selection Id",
                                   "format": "int64"
                                 },
                                 "name": {
                                   "type": "string",
                                   "description": "Selection name",
                                   "nullable": true
                                 },
                                 "probability": {
                                   "type": "number",
                                   "description": "Selection probability",
                                   "format": "float",
                                   "nullable": true
                                 },
                                 "price": {
                                   "type": "number",
                                   "description": "Selection odds",
                                   "format": "float",
                                   "nullable": true
                                 },
                                 "originalPrice": {
                                   "type": "number",
                                   "description": "Selection odds",
                                   "format": "float",
                                   "nullable": true
                                 },
                                 "status": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Enums.TradingStatus"
                                     }
                                   ],
                                   "description": "Trading selection status\n\n0 = Open (Trading is active)\n\n1 = Suspended (Trading is suspended)\n\n2 = Closed (Trading is stopped)\n\n3 = Inactive (Trading is not configured)",
                                   "x-enumNames": [
                                     "Open",
                                     "Suspended",
                                     "Closed",
                                     "Inactive"
                                   ],
                                   "x-enumDescriptions": [
                                     "Trading is active",
                                     "Trading is suspended",
                                     "Trading is stopped",
                                     "Trading is not configured"
                                   ]
                                 },
                                 "settlement": {
                                   "allOf": [
                                     {
                                       "$ref": "#/components/schemas/TradeArt.TradeArtApi.Contracts.DeltaMessage.Enums.SettlementResult"
                                     }
                                   ],
                                   "description": "Settlement result of the selection\n\n0 = None (Selection is not settled)\n\n1 = Refund (Refund all bets)\n\n2 = Lose (Selection is lost)\n\n3 = Win (Selection is won)\n\n4 = HalfLose (Selection is half lost)\n\n5 = HalfWin (Selection is half won)",
                                   "x-enumNames": [
                                     "None",
                                     "Refund",
                                     "Lose",
                                     "Win",
                                     "HalfLose",
                                     "HalfWin"
                                   ],
                                   "x-enumDescriptions": [
                                     "Selection is not settled",
                                     "Refund all bets",
                                     "Selection is lost",
                                     "Selection is won",
                                     "Selection is half lost",
                                     "Selection is half won"
                                   ]
                                 },
                                 "deadHeatFactor": {
                                   "type": "number",
                                   "description": "Dead heat factor is a floating point number used when there is a tie in outrights",
                                   "format": "double",
                                   "nullable": true
                                 },
                                 "extraParameters": {
                                   "type": "object",
                                   "additionalProperties": {
                                     "type": "string"
                                   },
                                   "description": "Additional selection information",
                                   "nullable": true
                                 }
                               },
                               "additionalProperties": false,
                               "description": "Represents market selection information"
                             }
                           },
                           "securitySchemes": {
                             "Bearer": {
                               "type": "http",
                               "description": "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                               "scheme": "Bearer",
                               "bearerFormat": "JWT"
                             }
                           }
                         }
                       }
                       """;
    }
}
