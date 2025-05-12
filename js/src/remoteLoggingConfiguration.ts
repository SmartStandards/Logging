
//TODO: diese struktur kann genau so aus dem packet heraus in die UShell-Konfiguration eingehangen werden
//      damit die ushell identisch zu serverseitigen variante(unter asp) ihre logging-config bekommt

export class RemoteLoggingConfiguration {
  
  public TargetSinkUrl: string|null = null;

  public AuthHeader: string | null = null;

  /// <summary>
  /// Names of additional (ambient-)metadata which should be added to the log entires.
  /// For example "SystemName" "InfrastructureZone" "PortfolioName" or any additinal information
  /// corresponding to the current application-scope.
  /// </summary>
  public Enrichments : string[] = [];

}
