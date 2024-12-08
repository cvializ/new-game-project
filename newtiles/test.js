import { Subject, of, zip, from, mergeMap, map } from 'rxjs';

const heights$ = from(Array.from({ length: 10 }).map((unusedItem, index) => index));

const vertices$ = of(0, 1, 2, 3, 4, 5, 6);

vertices$.pipe(
    mergeMap((vertex) => heights$.pipe(map(height => [vertex, height])))
).subscribe(console.log);
