<?php
namespace DevExpress\Logify\Collectors;

use DevExpress\Logify\Core\iCollector;

class UserAgentCollector implements iCollector {

    #region iCollector Members
    function DataName() {
        return 'useragent';
    }
    function CollectData() {
        if (key_exists('HTTP_USER_AGENT', $_SERVER)) {
            return $_SERVER['HTTP_USER_AGENT'];
        }
        return null;
    }
    #endregion
}
?>