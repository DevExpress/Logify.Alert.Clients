<?php
namespace DevExpress\Logify\Collectors;

use DevExpress\Logify\Core\iCollector;

class PlatformCollector implements iCollector {

    #region iCollector Members
    function DataName() {
        return 'platform';
    }
    function CollectData() {
        return 'PHP';
    }
    #endregion
}
?>