<?php
namespace DevExpress\Logify\Collectors;

use DevExpress\Logify\Core\iCollector;

class PHPVersionCollector implements iCollector {

    #region iCollector Members
    function DataName() {
        return 'phpversion';
    }
    function CollectData() {
        return PHP_VERSION;
    }
    #endregion
}
?>