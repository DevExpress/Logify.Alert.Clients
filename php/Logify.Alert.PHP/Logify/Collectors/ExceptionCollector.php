<?php
namespace DevExpress\Logify\Collectors;

use DevExpress\Logify\Core\iCollector;

class ExceptionCollector implements iCollector {

    public $exceptions = array();
    #region iCollector Members
    function DataName() {
        return 'exception';
    }
    public function CollectData() {
        $result = array();
        foreach ($this->exceptions as $e) {
            $stackTrace = $e->getTraceAsString();
            $severity = 'undefined';
            if ($e instanceof \ErrorException) {
                $severity = $e->getSeverity();
            }
            $result[] = array(
                'type' => get_class($e),
                'message' => $e->getMessage(),
                'code' => $e->getCode(),
                'file' => $e->getFile(),
                'line' => $e->getLine(),
                'severity' => $severity,
                'stackTrace' => $stackTrace,
                'normalizedStackTrace' => $stackTrace,
            );
        }
        return $result;
    }
    #endregion
    public function AddException($e) {
        $this->exceptions[] = $e;
    }
    public static function GetInstance($e) {
        $result = new ExceptionCollector();
        $result->AddException($e);
        return $result;
    }
}
?>