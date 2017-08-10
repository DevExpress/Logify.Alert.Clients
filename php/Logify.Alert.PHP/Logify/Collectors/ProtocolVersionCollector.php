<?php
namespace DevExpress\Logify\Collectors;

use DevExpress\Logify\Core\iCollector;

class ProtocolVersionCollector implements iCollector {

    #region iCollector Members
    function DataName() {
        return 'logifyProtocolVersion';
    }
    function CollectData() {
        return '17.1';
    }
    #endregion
}
?>