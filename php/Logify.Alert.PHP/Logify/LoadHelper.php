<?php
namespace DevExpress;

class LoadHelper {

    public static function LoadModule($className) {
        $namespaceName = 'DevExpress\\Logify\\';
        $namespaceNamePos = strpos($className, $namespaceName);
        if ($namespaceNamePos === 0) {
            $subFolderPath = substr($className, $namespaceNamePos + strlen($namespaceName));
            $classPath = $subFolderPath . '.php';
            $filePath = __DIR__ . DIRECTORY_SEPARATOR . $classPath;
            require_once($filePath);
        }
    }
}
?>