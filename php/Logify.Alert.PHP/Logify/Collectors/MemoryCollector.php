<?php
namespace DevExpress\Logify\Collectors;

use DevExpress\Logify\Core\iCollector;

class MemoryCollector implements iCollector {

    #region iCollector Members
    function DataName() {
        return 'memory';
    }
    public function CollectData() {
        $bytes = memory_get_usage();
        $mBytes = number_format($bytes / 1048576, 2, '.', '');
        $result = sprintf('%1$s Mb (%2$s bytes).', $mBytes, $bytes);
        return array(
            'workingSet' => $result,
        );
    }
    #endregion
}
?>