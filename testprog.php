<?php
{
    function fib($a){
        if($a < 2){
            return $a;
        }else{
            return fib($a-1)+fib($a-2);
        }        
    }
    echo fib(6);
}
?>