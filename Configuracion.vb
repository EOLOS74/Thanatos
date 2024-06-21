Imports System.Net
Imports System.Net.Mail

Module Configuracion

    Public ReadOnly Provincias As New Dictionary(Of String, String) From {
        {"03", "ALICANTE"},
        {"05", "AVILA"},
        {"06", "BADAJOZ"},
        {"08", "BARCELONA"},
        {"10", "CACERES"},
        {"11", "CADIZ"},
        {"21", "HUELVA"},
        {"23", "JAEN"},
        {"24", "LEON"},
        {"28", "MADRID"},
        {"35", "LAS PALMAS"},
        {"37", "SALAMANCA"},
        {"38", "TENERIFE"},
        {"41", "SEVILLA"},
        {"46", "VALENCIA"},
        {"49", "ZAMORA"},
        {"51", "CEUTA"}
    }

    Public ReadOnly emailAltaNueva As New Dictionary(Of String, String) From {
        {"03", "diego.leal@atelcosoluciones.es;fernandoluis.rus@atelcosoluciones.es;miguel.vega@atelcosoluciones.es;si.soporte@atelcosoluciones.es"},
        {"05", "eva.simarro@atelcosoluciones.es;ivan.palencia@atelcosoluciones.es;sergio.fernandez@atelcosoluciones.es;manuel.torrado@atelcosoluciones.es;si.soporte@atelcosoluciones.es;alberto.arteaga@atelcosoluciones.es;mario.ibanez@atelcosoluciones.es"},
        {"06", "antonio.bayon@atelcosoluciones.es;ivan.martinez@atelcosoluciones.es;si.soporte@atelcosoluciones.es;juanantonio.bastida@atelcosoluciones.es"},
        {"08", "si.soporte@atelcosoluciones.es"},
        {"10", "jesus.ayuela@atelcosoluciones.es;eusebio.bermejo@atelcosoluciones.es;si.soporte@atelcosoluciones.es"},
        {"11", "mario.ibanez@atelcosoluciones.es;ivangaspar.alonso@atelcosoluciones.es;antonio.marchena@atelcosoluciones.es;franciscojavier.silva@atelcosoluciones.es;si.soporte@atelcosoluciones.es"},
        {"21", "angelluis.lara@atelcosoluciones.es;juanjose.franco@atelcosoluciones.es;juanjose.beltran@atelcosoluciones.es;joseantonio.costa@atelcosoluciones.es;si.soporte@atelcosoluciones.es"},
        {"23", "javier.armenteros@atelcosoluciones.es;fernandoluis.rus@atelcosoluciones.es;si.soporte@atelcosoluciones.es"},
        {"24", "eva.simarro@atelcosoluciones.es;ivan.rouco@atelcosoluciones.es;sergio.fernandez@atelcosoluciones.es;si.soporte@atelcosoluciones.es"},
        {"28", "ivan.palencia@atelcosoluciones.es;sergio.fernandez@atelcosoluciones.es;eva.simarro@atelcosoluciones.es;si.soporte@atelcosoluciones.es;alberto.arteaga@atelcosoluciones.es"},
        {"35", "javier.armenteros@atelcosoluciones.es;joseluis.diaz@atelcosoluciones.es;si.soporte@atelcosoluciones.es;juanjose.beltran@atelcosoluciones.es"},
        {"37", "sergio.fernandez@atelcosoluciones.es;sebastian.viedma@atelcosoluciones.es;joseluis.diaz@atelcosoluciones.es;si.soporte@atelcosoluciones.es"},
        {"38", "si.soporte@atelcosoluciones.es"},
        {"41", "mario.ibanez@atelcosoluciones.es;si.soporte@atelcosoluciones.es"},
        {"46", "si.soporte@atelcosoluciones.es"},
        {"49", "sergio.fernandez@atelcosoluciones.es;luis.quer@atelcosoluciones.es;sebastian.viedma@atelcosoluciones.es;si.soporte@atelcosoluciones.es"},
        {"51", "ivangaspar.alonso@atelcosoluciones.es;antonio.marchena@atelcosoluciones.es;si.soporte@atelcosoluciones.es"},
        {"TODAS", "si.soporte@atelcosoluciones.es"}
    }

    Public ReadOnly emailBaja As New Dictionary(Of String, String) From {
        {"03", "diego.leal@atelcosoluciones.es;fernandoluis.rus@atelcosoluciones.es;miguel.vega@atelcosoluciones.es;si.soporte@atelcosoluciones.es"},
        {"05", "eva.simarro@atelcosoluciones.es;ivan.palencia@atelcosoluciones.es;sergio.fernandez@atelcosoluciones.es;manuel.torrado@atelcosoluciones.es;si.soporte@atelcosoluciones.es;alberto.arteaga@atelcosoluciones.es;mario.ibanez@atelcosoluciones.es"},
        {"06", "antonio.bayon@atelcosoluciones.es;ivan.martinez@atelcosoluciones.es;si.soporte@atelcosoluciones.es;juanantonio.bastida@atelcosoluciones.es"},
        {"08", "si.soporte@atelcosoluciones.es"},
        {"10", "jesus.ayuela@atelcosoluciones.es;eusebio.bermejo@atelcosoluciones.es;si.soporte@atelcosoluciones.es"},
        {"11", "mario.ibanez@atelcosoluciones.es;ivangaspar.alonso@atelcosoluciones.es;antonio.marchena@atelcosoluciones.es;franciscojavier.silva@atelcosoluciones.es;si.soporte@atelcosoluciones.es"},
        {"21", "angelluis.lara@atelcosoluciones.es;juanjose.franco@atelcosoluciones.es;juanjose.beltran@atelcosoluciones.es;joseantonio.costa@atelcosoluciones.es;si.soporte@atelcosoluciones.es"},
        {"23", "javier.armenteros@atelcosoluciones.es;fernandoluis.rus@atelcosoluciones.es;si.soporte@atelcosoluciones.es"},
        {"24", "eva.simarro@atelcosoluciones.es;ivan.rouco@atelcosoluciones.es;sergio.fernandez@atelcosoluciones.es;si.soporte@atelcosoluciones.es"},
        {"28", "ivan.palencia@atelcosoluciones.es;sergio.fernandez@atelcosoluciones.es;eva.simarro@atelcosoluciones.es;si.soporte@atelcosoluciones.es;alberto.arteaga@atelcosoluciones.es"},
        {"35", "javier.armenteros@atelcosoluciones.es;joseluis.diaz@atelcosoluciones.es;si.soporte@atelcosoluciones.es;juanjose.beltran@atelcosoluciones.es"},
        {"37", "sergio.fernandez@atelcosoluciones.es;sebastian.viedma@atelcosoluciones.es;joseluis.diaz@atelcosoluciones.es;si.soporte@atelcosoluciones.es"},
        {"38", "si.soporte@atelcosoluciones.es"},
        {"41", "mario.ibanez@atelcosoluciones.es;si.soporte@atelcosoluciones.es"},
        {"46", "si.soporte@atelcosoluciones.es"},
        {"49", "sergio.fernandez@atelcosoluciones.es;luis.quer@atelcosoluciones.es;sebastian.viedma@atelcosoluciones.es;si.soporte@atelcosoluciones.es"},
        {"51", "ivangaspar.alonso@atelcosoluciones.es;antonio.marchena@atelcosoluciones.es;si.soporte@atelcosoluciones.es"},
        {"TODAS", "si.soporte@atelcosoluciones.es"}
    }

    Public ReadOnly emailAltaTelco As New Dictionary(Of String, String) From {
        {"03", "diego.leal@atelcosoluciones.es;fernandoluis.rus@atelcosoluciones.es;miguel.vega@atelcosoluciones.es;si.soporte@atelcosoluciones.es"},
        {"05", "eva.simarro@atelcosoluciones.es;ivan.palencia@atelcosoluciones.es;sergio.fernandez@atelcosoluciones.es;manuel.torrado@atelcosoluciones.es;si.soporte@atelcosoluciones.es;alberto.arteaga@atelcosoluciones.es;mario.ibanez@atelcosoluciones.es"},
        {"06", "antonio.bayon@atelcosoluciones.es;ivan.martinez@atelcosoluciones.es;si.soporte@atelcosoluciones.es;juanantonio.bastida@atelcosoluciones.es"},
        {"08", "si.soporte@atelcosoluciones.es"},
        {"10", "jesus.ayuela@atelcosoluciones.es;eusebio.bermejo@atelcosoluciones.es;si.soporte@atelcosoluciones.es"},
        {"11", "mario.ibanez@atelcosoluciones.es;ivangaspar.alonso@atelcosoluciones.es;antonio.marchena@atelcosoluciones.es;franciscojavier.silva@atelcosoluciones.es;si.soporte@atelcosoluciones.es"},
        {"21", "angelluis.lara@atelcosoluciones.es;juanjose.franco@atelcosoluciones.es;juanjose.beltran@atelcosoluciones.es;joseantonio.costa@atelcosoluciones.es;si.soporte@atelcosoluciones.es"},
        {"23", "javier.armenteros@atelcosoluciones.es;fernandoluis.rus@atelcosoluciones.es;si.soporte@atelcosoluciones.es"},
        {"24", "eva.simarro@atelcosoluciones.es;ivan.rouco@atelcosoluciones.es;sergio.fernandez@atelcosoluciones.es;si.soporte@atelcosoluciones.es"},
        {"28", "ivan.palencia@atelcosoluciones.es;sergio.fernandez@atelcosoluciones.es;eva.simarro@atelcosoluciones.es;si.soporte@atelcosoluciones.es;alberto.arteaga@atelcosoluciones.es"},
        {"35", "javier.armenteros@atelcosoluciones.es;joseluis.diaz@atelcosoluciones.es;si.soporte@atelcosoluciones.es;juanjose.beltran@atelcosoluciones.es"},
        {"37", "sergio.fernandez@atelcosoluciones.es;sebastian.viedma@atelcosoluciones.es;joseluis.diaz@atelcosoluciones.es;si.soporte@atelcosoluciones.es"},
        {"38", "si.soporte@atelcosoluciones.es"},
        {"41", "mario.ibanez@atelcosoluciones.es;si.soporte@atelcosoluciones.es"},
        {"46", "si.soporte@atelcosoluciones.es"},
        {"49", "sergio.fernandez@atelcosoluciones.es;luis.quer@atelcosoluciones.es;sebastian.viedma@atelcosoluciones.es;si.soporte@atelcosoluciones.es"},
        {"51", "ivangaspar.alonso@atelcosoluciones.es;antonio.marchena@atelcosoluciones.es;si.soporte@atelcosoluciones.es"},
        {"TODAS", "si.soporte@atelcosoluciones.es"}
    }

    Public ReadOnly gestorPadreTecnico As New Dictionary(Of String, String) From {
        {"03", "PAORG0017415"},
        {"05", "PAORG0017572"},
        {"06", "PAORG0017417"},
        {"10", "PAORG0023190"},
        {"11", "PAORG0017424"},
        {"21", "PAORG0032562"},
        {"23", "PAORG0017426"},
        {"24", "PAORG0059556"},
        {"28", "PAORG0017407"},
        {"35", "PAORG0023198"},
        {"37", "PAORG0059559"},
        {"41", "PAORG0017430"},
        {"49", "PAORG0059561"},
        {"51", "PAORG0017434"}
    }

    Public ReadOnly gestorPadreGestor As New Dictionary(Of String, String) From {
        {"03", "PAORG0017416"},
        {"05", "PAORG0023187"},
        {"06", "PAORG0017418"},
        {"10", "PAORG0038328"},
        {"11", "PAORG0017425"},
        {"21", "PAORG0364310"},
        {"23", "PAORG0017427"},
        {"24", "PAORG0059557"},
        {"28", "PAORG0017408"},
        {"35", "PAORG0059555"},
        {"37", "PAORG0023181"},
        {"41", "PAORG0017431"},
        {"49", "PAORG0059560"},
        {"51", "PAORG0017425"}
    }

    Public Enum EstadoCheck
        Nada
        Pendiente
        Correcto
        Fallido
    End Enum

End Module